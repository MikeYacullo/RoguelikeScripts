using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class GameController : MonoBehaviour
{

	public tk2dTileMap tileMapTerrain;
	
	public tk2dTileMap tileMapItems;
	
	private tk2dCamera camera;

	private Map[] levels;
	private int currentLevel = 0;
	private int previousLevel = -1;
	private Map map;
	
	private enum GameState
	{
		Initializing
		,
		LevelTransition
		,
		TurnPlayer
		,
		TurnEnemyInProgress
		,
		TurnEnemy
		,
		PlayerDeath
	}
	
	private GameState gameState;
	
	private PlayerCharacter pc;
	private int pcSpriteId;
	private tk2dTileMap tileMapCharacters;
	private bool pcIsFlipped = false;
	
	private UnityEngine.UI.Text txtMessages;
	List<string> messages = new List<string> ();
	int MAX_MESSAGE_COUNT = 5;
	
	private GameObject pnlTransition;
	private UnityEngine.UI.Text txtTransition;
	
	private float SECONDS_BETWEEN_TURNS = 0.1f;
	
	private UnityEngine.UI.Image health1, health2, health3, health4;
	public Sprite spriteHeartFull;
	public Sprite spriteHeartHalf;
	public Sprite spriteHeartEmpty;

	private int mapWidth = 19, mapHeight = 19;
	
	private int cameraScaleFactor = 24;
	
	public float VOLUME = 1.0f;
	public AudioClip audioStep;
	public AudioClip audioDoor;
	public AudioClip audioHitEnemy;
	public AudioClip audioHitPlayer;
	public AudioClip audioWhiff;
	public AudioClip audioDieEnemy;
	
	private List<Enemy> enemies = new List<Enemy> ();
	private List<Enemy>[] levelEnemies;
	
	private List<Item> items = new List<Item> ();
	private List<Item>[] levelItems;
	private List<Type>[] levelItemTypes;
	
	int LEVEL_COUNT = 5;
	//tilemap constants
	int LAYER_FLOOR_AND_WALLS = 0;
	int LAYER_DOORS_AND_STAIRS = 1;
	int LAYER_VISIBILITY = 2;
	int TILE_FLOORDEFAULT;
	int TILE_DOORCLOSED;
	int TILE_DOOROPEN;
	int TILE_STAIRSUP;
	int TILE_STAIRSDOWN;
	int TILE_SOLIDBLACK;
	
	void Start ()
	{
		NewGame ();
	}
	
	void NewGame ()
	{
		Debug.Log ("new game!");
		gameState = GameState.Initializing;
		camera = GameObject.Find ("camera").GetComponent<tk2dCamera> ();
		tileMapCharacters = GameObject.Find ("TileMapCharacters").GetComponent<tk2dTileMap> ();
		tileMapItems = GameObject.Find ("TileMapItems").GetComponent<tk2dTileMap> ();
		txtMessages = GameObject.Find ("txtMessages").GetComponent<UnityEngine.UI.Text> ();
		
		pnlTransition = GameObject.Find ("pnlTransition");
		txtTransition = GameObject.Find ("txtTransition").GetComponent<UnityEngine.UI.Text> ();
		pnlTransition.SetActive (false);
		
		health1 = GameObject.Find ("health1").GetComponent<UnityEngine.UI.Image> ();
		health2 = GameObject.Find ("health2").GetComponent<UnityEngine.UI.Image> ();
		health3 = GameObject.Find ("health3").GetComponent<UnityEngine.UI.Image> ();
		health4 = GameObject.Find ("health4").GetComponent<UnityEngine.UI.Image> ();
		levels = new Map[LEVEL_COUNT];
		levelEnemies = new List<Enemy>[LEVEL_COUNT];
		levelItems = new List<Item>[LEVEL_COUNT];
		for (int i=0; i<LEVEL_COUNT; i++) {
			levels [i] = new Map (mapWidth, mapHeight);
		}
		
		levelItemTypes = new List<Type>[LEVEL_COUNT];
		levelItemTypes [0] = new List<Type> {typeof(PotionRed_S), typeof(Gold_S)};
		
		DisplayMessage ("Welcome.");
		CreatePlayerCharacter ();
		currentLevel = -1;
		MoveToLevel (0);
	}
	
	private void MoveToLevel (int level)
	{
		gameState = GameState.LevelTransition;
		txtTransition.text = "Level " + level.ToString ();
		pnlTransition.SetActive (true);
		StartCoroutine (WaitAndMove (level));
	}
	
	IEnumerator WaitAndMove (int level)
	{
		yield return new WaitForSeconds (1.0f);
		SaveLevelState ();
		previousLevel = currentLevel;
		currentLevel = level;
		InitMap ();
		InitPlayerCharacter ();
		InitEnemies ();
		InitItems ();
		UpdateHud ();
		pnlTransition.SetActive (false);
		gameState = GameState.TurnPlayer;
		//Debug.Log ("init level " + currentLevel + " complete");
	}

	private void SaveLevelState ()
	{
		if (currentLevel != -1) {
			levelEnemies [currentLevel] = enemies;
		}
	}
	
	void InitItems ()
	{
		for (int i=0; i<10; i++) {
			//pick an item from the list
			int itemNum = UnityEngine.Random.Range (0, levelItemTypes [currentLevel].Count);
			
			Type t = levelItemTypes [currentLevel] [itemNum];
			Item item;
			item = (Item)Activator.CreateInstance (t);
			item.Location = map.GetRandomCell (true);
			items.Add (item);
		}
		RenderItems ();
	}
	
	private void RenderItems ()
	{
		//clear the whole thing
		for (int w=0; w<mapWidth; w++) {
			for (int h=0; h<mapHeight; h++) {
				if (map.Cells [w, h].Visited) {
					tileMapItems.ClearTile (w, h, 0);
				}
			}
		}
		//draw items
		for (int i=0; i<items.Count; i++) {
			if (map.Cells [items [i].Location.x, items [i].Location.y].Visited) {
				//TODO optimize this!
				int spriteId = tileMapItems.SpriteCollectionInst.GetSpriteIdByName (items [i].SpriteName);
				tileMapItems.SetTile (items [i].Location.x, items [i].Location.y, 0, spriteId);
			}
		}
		tileMapItems.Build ();
	}

	void InitMap ()
	{
		/*
		if (previousLevel != -1) {
			levels [previousLevel] = map;
		}*/
		map = levels [currentLevel];
		RenderMap ();
	}
	
	void CreatePlayerCharacter ()
	{
		string className = PlayerPrefs.GetString ("className");
		bool isMale;
		PlayerCharacter.ClassType classType = PlayerCharacter.ClassType.None;
		string[] classInfo = className.Split ('_');
		if (classInfo [1] == "m") {
			isMale = true;
		} else {
			isMale = false;
		}
		switch (classInfo [0]) {
		case "cleric":
			classType = PlayerCharacter.ClassType.Cleric;
			break;
		case "fighter":
			classType = PlayerCharacter.ClassType.Fighter;
			break;
		case "rogue":
			classType = PlayerCharacter.ClassType.Rogue;
			break;
		case "wizard":
			classType = PlayerCharacter.ClassType.Wizard;
			break;
		default:
			break;
		}
		pc = new PlayerCharacter (classType, isMale);
		InitCharacterSprite ();
		//Debug.Log (pc.classType.ToString ());
	}
	
	void InitPlayerCharacter ()
	{
		if (currentLevel > previousLevel) {
			//coming down stairs
			MovePlayerTo (map.entranceLocation);
			Debug.Log ("moving to entrance");
		} else {
			//going up stairs
			MovePlayerTo (map.exitLocation);
		}
		SeeTilesFlood ();
	}
	
	void InitEnemies ()
	{
		if (levelEnemies [currentLevel] != null) {
			enemies = levelEnemies [currentLevel];
		} else {
			enemies = new List<Enemy> ();
			for (int i=0; i<3; i++) {
				Enemy_Bat enemy = new Enemy_Bat (map.GetRandomCell (true));
				enemies.Add (enemy);
			}
		
			for (int i=0; i<3; i++) {
				Enemy_Spider enemy = new Enemy_Spider (map.GetRandomCell (true));
				enemies.Add (enemy);
			}
			for (int i=0; i<3; i++) {
				Enemy_GreenSlime enemy = new Enemy_GreenSlime (map.GetRandomCell (true));
				enemies.Add (enemy);
			}
		
		}
		RenderTMCharacters ();
	}
	
	
	
	void InitCharacterSprite ()
	{
		string sex;
		if (pc.IsMale) {
			sex = "_m";
		} else {
			sex = "_f";
		}
		switch (pc.classType) {
		case PlayerCharacter.ClassType.Cleric:
			pcSpriteId = tileMapCharacters.SpriteCollectionInst.GetSpriteIdByName ("cleric" + sex);
			break;
		case PlayerCharacter.ClassType.Fighter:
			pcSpriteId = tileMapCharacters.SpriteCollectionInst.GetSpriteIdByName ("fighter" + sex);
			break;
		case PlayerCharacter.ClassType.Rogue:
			pcSpriteId = tileMapCharacters.SpriteCollectionInst.GetSpriteIdByName ("rogue" + sex);
			break;
		case PlayerCharacter.ClassType.Wizard:
			pcSpriteId = tileMapCharacters.SpriteCollectionInst.GetSpriteIdByName ("wizard" + sex);
			break;		
		}
		//Debug.Log (pcSpriteId);
	}
	
	void RenderMap ()
	{
		tileMapTerrain = GameObject.Find ("TileMapTerrain").GetComponent<tk2dTileMap> ();
		if (tileMapTerrain == null) {
			Debug.Log ("can't find terrain");
		}
		//init "constants"
		TILE_DOORCLOSED = tileMapTerrain.SpriteCollectionInst.GetSpriteIdByName ("doorClosed");
		TILE_DOOROPEN = tileMapTerrain.SpriteCollectionInst.GetSpriteIdByName ("doorOpen");
		TILE_STAIRSUP = tileMapTerrain.SpriteCollectionInst.GetSpriteIdByName ("stairsUp");
		TILE_STAIRSDOWN = tileMapTerrain.SpriteCollectionInst.GetSpriteIdByName ("stairsDown");
		TILE_FLOORDEFAULT = tileMapTerrain.SpriteCollectionInst.GetSpriteIdByName ("floor0");
		TILE_SOLIDBLACK = tileMapTerrain.SpriteCollectionInst.GetSpriteIdByName ("solidBlack");
		//loop through map
		for (int h=0; h<map.Height; h++) {
			for (int w = 0; w<map.Width; w++) {
				//hide the whole map to start
				tileMapCharacters.ClearTile (w, h, 0);
				tileMapTerrain.ClearTile (w, h, LAYER_FLOOR_AND_WALLS);
				tileMapTerrain.ClearTile (w, h, LAYER_DOORS_AND_STAIRS);
				tileMapTerrain.SetTile (w, h, LAYER_VISIBILITY, TILE_SOLIDBLACK);
				switch (map.Cells [w, h].Type) {
				case Map.CellType.Door:
					//all doors start closed
					tileMapTerrain.SetTile (w, h, LAYER_FLOOR_AND_WALLS, TILE_FLOORDEFAULT);
					tileMapTerrain.SetTile (w, h, LAYER_DOORS_AND_STAIRS, TILE_DOORCLOSED);
					break;
				case Map.CellType.Entrance:
					//TODO logic here for top level
					tileMapTerrain.SetTile (w, h, LAYER_FLOOR_AND_WALLS, TILE_FLOORDEFAULT);
					tileMapTerrain.SetTile (w, h, LAYER_DOORS_AND_STAIRS, TILE_STAIRSUP);
					map.entranceLocation = new Address (w, h);
					break;
				case Map.CellType.Exit:
					//TODO logic here for bottom level
					tileMapTerrain.SetTile (w, h, LAYER_FLOOR_AND_WALLS, TILE_FLOORDEFAULT);
					tileMapTerrain.SetTile (w, h, LAYER_DOORS_AND_STAIRS, TILE_STAIRSDOWN);
					map.exitLocation = new Address (w, h);
					break;
				case Map.CellType.Floor:
					//randomize floor title a little
					int floorTag = UnityEngine.Random.Range (0, 3);
					string floorName = "floor" + floorTag;
					int floorId = tileMapTerrain.SpriteCollectionInst.GetSpriteIdByName (floorName);
					tileMapTerrain.SetTile (w, h, LAYER_FLOOR_AND_WALLS, floorId);
					break;
				case Map.CellType.Wall:
					string wallName = GetWallTileName (w, h);
					int wallId = tileMapTerrain.SpriteCollectionInst.GetSpriteIdByName (wallName);
					tileMapTerrain.SetTile (w, h, LAYER_FLOOR_AND_WALLS, wallId);
					break;
				default:
					//Debug.Log ("Can't find cell type " + map.Cells [w, h].Type.ToString ());
					break;
				}
			}
		}
		tileMapCharacters.Build ();
		tileMapTerrain.Build ();
	}

	// Update is called once per frame
	void Update ()
	{
		switch (gameState) {
		case GameState.TurnPlayer:
			TakePlayerTurn ();
			break;
		case GameState.TurnEnemy:
			gameState = GameState.TurnEnemyInProgress;
			StartCoroutine (WaitForSecondsThenExecute (SECONDS_BETWEEN_TURNS, () => TakeEnemyTurn ()));
			break;
		}
	}
	
	public IEnumerator WaitForSecondsThenExecute (float waitTime, Action method)
	{
		yield return new  WaitForSeconds (waitTime);
		method ();
	}
	
	private void UpdateHud ()
	{
		//update hearts
		float healthFraction = ((float)pc.CurrentHealth / (float)pc.MaxHealth);
		//Debug.Log ("Health:" + healthFraction);
		if (healthFraction <= 0) {
			health4.sprite = spriteHeartEmpty;
		}
		if (healthFraction >= 0.125f) {
			health4.sprite = spriteHeartHalf;
		}
		if (healthFraction >= 0.25f) {
			health4.sprite = spriteHeartFull;
		}
		if (healthFraction <= 0 + 0.25f) {
			health3.sprite = spriteHeartEmpty;
		}
		if (healthFraction >= 0.125f + 0.25f) {
			health3.sprite = spriteHeartHalf;
		}
		if (healthFraction >= 0.25f + 0.25f) {
			health3.sprite = spriteHeartFull;
		}
		if (healthFraction <= 0 + 0.50f) {
			health2.sprite = spriteHeartEmpty;
		}
		if (healthFraction >= 0.125f + 0.50f) {
			health2.sprite = spriteHeartHalf;
		}
		if (healthFraction >= 0.25f + 0.50f) {
			health2.sprite = spriteHeartFull;
		}
		if (healthFraction <= 0 + 0.75f) {
			health1.sprite = spriteHeartEmpty;
		}
		if (healthFraction >= 0.125f + 0.75f) {
			health1.sprite = spriteHeartHalf;
		}
		if (healthFraction >= 0.25f + 0.75f) {
			health1.sprite = spriteHeartFull;
		}
	}
	
	private void TakePlayerTurn ()
	{
		//clear current pc cell
		tileMapCharacters.ClearTile (pc.Location.x, pc.Location.y, 0);
		CheckInput ();
		//draw the character at its location
		tileMapCharacters.SetTile (pc.Location.x, pc.Location.y, 0, pcSpriteId);
		if (pcIsFlipped) {
			tileMapCharacters.SetTileFlags (pc.Location.x, pc.Location.y, 0, tk2dTileFlags.FlipX);
		}
		tileMapCharacters.Build ();
		RenderItems ();
		camera.transform.position = new Vector3 (pc.Location.x * cameraScaleFactor, pc.Location.y * cameraScaleFactor, -10);		
	}
	
	private void TakeEnemyTurn ()
	{
		for (int i=0; i<enemies.Count; i++) {
			if (map.DistanceToPlayer (enemies [i].Location) == 1) {
				CombatCheck (enemies [i], pc);
			} else {
				enemies [i].Move (map);
			}
		}
		RenderTMCharacters ();
		RenderItems ();
		UpdateHud ();
		gameState = GameState.TurnPlayer;
	}
	
	private void RenderTMCharacters ()
	{
		//clear the whole thing
		for (int w=0; w<mapWidth; w++) {
			for (int h=0; h<mapHeight; h++) {
				if (map.Cells [w, h].Visited) {
					tileMapCharacters.ClearTile (w, h, 0);
				}
			}
		}
		//draw pc
		tileMapCharacters.SetTile (pc.Location.x, pc.Location.y, 0, pcSpriteId);
		if (pcIsFlipped) {
			tileMapCharacters.SetTileFlags (pc.Location.x, pc.Location.y, 0, tk2dTileFlags.FlipX);
		}
		//draw enemies
		for (int i=0; i<enemies.Count; i++) {
			if (map.Cells [enemies [i].Location.x, enemies [i].Location.y].Visited) {
				//TODO optimize this!
				int spriteId = tileMapCharacters.SpriteCollectionInst.GetSpriteIdByName (enemies [i].SpriteName);
				tileMapCharacters.SetTile (enemies [i].Location.x, enemies [i].Location.y, 0, spriteId);
			}
		}
		tileMapCharacters.Build ();
	}

	
	private void CheckInput ()
	{
		if (Input.GetKeyDown (KeyCode.LeftBracket) && camera.ZoomFactor >= 1.0) {
			camera.ZoomFactor -= 0.5f;
		}
		if (Input.GetKeyDown (KeyCode.RightBracket) && camera.ZoomFactor <= 4.5) {
			camera.ZoomFactor += 0.5f;
		}
		bool isMoving = false;
		int newX = pc.Location.x, newY = pc.Location.y;
		if (Input.GetKeyDown (KeyCode.D) || Input.GetKeyDown (KeyCode.RightArrow)) {
			newX = newX + 1;
			pcIsFlipped = true;
			isMoving = true;
		} else if (Input.GetKeyDown (KeyCode.A) || Input.GetKeyDown (KeyCode.LeftArrow)) {
			newX = newX - 1;
			pcIsFlipped = false;
			isMoving = true;
		} else if (Input.GetKeyDown (KeyCode.W) || Input.GetKeyDown (KeyCode.UpArrow)) {
			newY = newY + 1;
			isMoving = true;
		} else if (Input.GetKeyDown (KeyCode.S) || Input.GetKeyDown (KeyCode.DownArrow)) {
			newY = newY - 1;
			isMoving = true;
		}
		if (isMoving) { 
			if (IsPassable (newX, newY)) {
				//open a closed door instead of moving in
				if (tileMapTerrain.GetTile (newX, newY, LAYER_DOORS_AND_STAIRS) == TILE_DOORCLOSED) {
					tileMapTerrain.SetTile (newX, newY, LAYER_DOORS_AND_STAIRS, TILE_DOOROPEN);
					tileMapTerrain.Build ();
					map.Cells [newX, newY].BlocksVision = false;
					audio.PlayOneShot (audioDoor, VOLUME);
				} else if (map.Cells [newX, newY].Type == Map.CellType.Exit && currentLevel != LEVEL_COUNT - 1) {
					map.Cells [pc.Location.x, pc.Location.y].Passable = true;
					MoveToLevel (currentLevel + 1);
				} else if (map.Cells [newX, newY].Type == Map.CellType.Entrance && currentLevel != 0) {
					map.Cells [pc.Location.x, pc.Location.y].Passable = true;
					MoveToLevel (currentLevel - 1);
				} else {
					MovePlayerTo (new Address (newX, newY));
					audio.PlayOneShot (audioStep, VOLUME);
				}
				SeeTilesFlood ();
			} else {
				//combat?
				//does the square contain an enemy?
				//TODO make map of enemies so we don't have to loop through
				int enemyIndex = -1;
				for (int i=0; i<enemies.Count; i++) {
					if (enemies [i].Location.x == newX && enemies [i].Location.y == newY) {
						enemyIndex = i;
						break;
					}
				}
				if (enemyIndex != -1) {
					CombatCheck (pc, enemies [enemyIndex]);
					//TODO encapsulate
					if (enemies [enemyIndex].CurrentHealth <= 0) {
						audio.PlayOneShot (audioDieEnemy, VOLUME);
						map.Cells [enemies [enemyIndex].Location.x, enemies [enemyIndex].Location.y].Passable = true;
						enemies.RemoveAt (enemyIndex);
					}
				}
			}
			UpdateHud ();
			gameState = GameState.TurnEnemy;
		}
	}
	
	private void MovePlayerTo (Address newLocation)
	{
		//old cell can now be walked through
		map.Cells [pc.Location.x, pc.Location.y].Passable = true;
		pc.Location = newLocation;
		//block character's current location
		map.Cells [pc.Location.x, pc.Location.y].Passable = false;
		map.pcLocation = newLocation;
	}
	
	private void CombatCheck (Actor attacker, Actor defender)
	{
		//Debug.Log ("combat check!");
		//TODO make actual combat system
		DisplayMessage (attacker.Name + " attacks " + defender.Name + "...");
		if (UnityEngine.Random.Range (1, 10) > 4) {
			//hit
			int damage = UnityEngine.Random.Range (1, attacker.AttackMaxDamage + 1);
			DisplayMessage (defender.Name + " is hit for " + damage + " damage!");
			defender.CurrentHealth -= damage;
			if (attacker.GetType ().ToString () == "PlayerCharacter") {
				audio.PlayOneShot (audioHitEnemy, VOLUME);
			} else {
				audio.PlayOneShot (audioHitPlayer, VOLUME);
			}
		} else {
			//miss
			DisplayMessage (attacker.Name + " misses!");
			audio.PlayOneShot (audioWhiff, 0.3f);
		}
	}
	
	private void DisplayMessage (string messageText)
	{
		//TODO eventually this will be displayed in the HUD
		Debug.Log (messageText);
		messages.Add (messageText);
		if (messages.Count > MAX_MESSAGE_COUNT) {
			messages.RemoveAt (0);
		}
		txtMessages.text = "";
		for (int i = 0; i<messages.Count; i++) {
			txtMessages.text += "\n" + messages [i];
		}
	}
	
	private bool IsPassable (int newX, int newY)
	{
		if (map.Cells [newX, newY].Passable) {
			return true;
		}
		return false;
	}
	
	private string GetWallTileName (int x, int y)
	{
		int goesN, goesE, goesS, goesW;
		//check each direction
		goesN = WallGoes (x, y + 1); //1
		goesE = WallGoes (x + 1, y);//2
		goesS = WallGoes (x, y - 1);//4
		goesW = WallGoes (x - 1, y);//8
		int wallNum = goesN + (goesE * 2) + (goesS * 4) + (goesW * 8);
		switch (wallNum) {
		case 0:
			return "pillar0";
		case 1:
			return "wallN";
		case 2:
			return "wallE";
		case 3:
			return "wallNE";
		case 4:
			return "wallS";
		case 5:
			return "wallNS";
		case 6:
			return "wallES";
		case 7:
			return "wallNES";
		case 8:
			return "wallW";
		case 9:
			return "wallNW";
		case 10:
			return "wallEW";
		case 11:
			return "wallNEW";
		case 12:
			return "wallSW";
		case 13:
			return "wallNSW";
		case 14:
			return "wallESW";
		case 15:
			return "wallNESW";			
		default:
			return "pillar1";			
		}
		
	}
	
	private int WallGoes (int x, int y)
	{
		if (x < 0 || x >= mapWidth || y < 0 || y >= mapHeight) {
			return 0;
		} else if (map.Cells [x, y].Type == Map.CellType.Wall) {
			return 1;
		} else {
			return 0; 
		}
	}
	
	private void SeeTilesFlood ()
	{
		bool newCellsSeen = false;
		List<Address> vTiles = map.findVisibleCellsFlood (new Address (pc.Location.x, pc.Location.y), pc.VisionRange);
		foreach (Address a in vTiles) {
			//if (!map.Cells [a.x, a.y].Visited) {
			map.Cells [a.x, a.y].Visited = true;
			tileMapTerrain.ClearTile (a.x, a.y, LAYER_VISIBILITY);
			newCellsSeen = true;
			//}
		}
		//if (newCellsSeen) {
		tileMapTerrain.Build ();
		//}
	}
	

	
	
	
}
