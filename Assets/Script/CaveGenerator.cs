using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
public class CaveGenerator : MonoBehaviour {

	// Use this for initialization
	int [,] 		caveMap;

	public int 		caveHeight;
	public int 		caveWidth;
	public int 		smallestRegion;


	[Range(0.0f,100.0f)]
	public float 	caveFillPrectage;

	[Range(0.0f, 20.0f)]
	public int      numOfSmooth;


	void Start () {
		
		//GenerateMap();
	}
	public void GenerateMap(){
		caveMap 			 = new int[caveWidth, caveHeight];
		//caveMap 			 = new int[caveWidth, caveHeight];


		for(int i = 0; i < caveWidth; i++){
			for (int j = 0; j < caveHeight; j++){

				if (i == 0 || j == 0 || i == caveWidth - 1 || j == caveHeight - 1) {
					caveMap [i, j] = 1;
				} else {
					int randomNum = UnityEngine.Random.Range(1,101);
					if (caveFillPrectage >= randomNum) {
						caveMap [i, j] = 1;
					} else {
						caveMap [i, j] = 0;
					}
				}
			}
		}
		SmoothMap ();
		MeshGenerator meshGen = GetComponent<MeshGenerator>();
		meshGen.GenerateMesh(caveMap, 1);


	}


	void SmoothMap(){
		for (int i = 0; i < caveWidth; i++) {
			for (int j = 0; j < caveHeight; j++) {
				int numOfWalls = CountForWalls (i, j);

				if(caveMap[i,j] == 1){
					if (numOfWalls > 3) {
						caveMap [i, j] = 1;
					} else if (numOfWalls < 2) {
						caveMap [i, j] = 0;
					}
				}else{
					if (numOfWalls > 4 ) {
						caveMap [i, j] = 1;
					} else {
						caveMap [i, j] = 0;
					}

				}

			}
		}
	}
		
	void Update() {
		
	}


	int CountForWalls(int i, int j){

		int numOfWalls = 0;

		int left  = i - 1;
		int right = i + 1;
		int up    = j - 1;
		int down  = j + 1;


		for(int startX = left; startX <= right; startX++){
			for(int startY = up; startY <= down; startY++){

				if (startX != i || startY != j) {
					if (isOutOfBounds(startX,startY)) {
						numOfWalls++;
					} else {
						numOfWalls += caveMap [startX, startY];
					}
				}


			}
		}
		return numOfWalls;
	} 

	List<CaveRoom> GetRoomList(){
		List<CaveRoom> roomList = new List<CaveRoom> ();
		int[,] mapFlags 		= new int[caveWidth, caveHeight];

		for(int i = 0; i < caveWidth; i++){
			for (int j = 0; j < caveHeight; j++) {
				if (mapFlags [i, j] == 0 && caveMap [i, j] == 0) {
					CaveRoom room = GetRoom (i, j);
					roomList.Add (room);

					foreach (Vector2 v in room.tiles) {
						mapFlags [(int)v.x, (int)v.y] = 1;
					}
				}
			}
		}
		return roomList;
	}


	CaveRoom GetRoom(int startX, int startY){
		CaveRoom room 	 	 = new CaveRoom ();
		int[,] mapFlags 	 = new int[caveWidth, caveHeight];
		Queue tileQueue 	 = new Queue ();
		int tileType 		 = caveMap [startX, startY];
		tileQueue.Enqueue (new Vector2 (startX, startY));
		while (tileQueue.Count > 0) {
			Vector2 tile = (Vector2) tileQueue.Dequeue ();
			int left 	 = (int)tile.x - 1;
			int right 	 = (int)tile.x + 1;
			int up 		 = (int)tile.y - 1;
			int down 	 = (int)tile.y + 1;

			room.tiles.Add (tile);
			for (int i = left; i <= right; i++) {
				for (int j = up; j <= down; j++) {
					if (!isOutOfBounds (i, j)) {
						if (i == tile.x || j == tile.y) {
							if (!isOutOfBounds (i, j) && mapFlags [i, j] == 0 && caveMap [i, j] == tileType) {
								mapFlags [i, j] = 1;
								tileQueue.Enqueue (new Vector2(i,j));
							}
						}
					}
				}
			}
		}

		room.createEdgeTiles (caveMap);

		return room;
	}



	bool isOutOfBounds(int x, int y){
		if (x < 0 || y < 0 || x >= caveWidth || y >= caveHeight) {
			return true;
		}
		return false;
	}

	class CaveRoom{
		private List<Vector2> _tiles;
		public List<Vector2> tiles{
			get { return _tiles;}
		}

		private List<Vector2> _edgeTitles;
		public List<Vector2> edgeTitles{
			get { return _edgeTitles; }
		}

		private List<CaveRoom> _connectRooms;
		public  List<CaveRoom> connectRooms{
			get { return _connectRooms; }
		}

		public CaveRoom(){
			_tiles        = new List<Vector2>();
			_edgeTitles	  = new List<Vector2>();
			_connectRooms = new List<CaveRoom>();
		}

		public CaveRoom(List<Vector2> tS, int[,] cMap){
			_tiles	  	  = tS;
			_connectRooms = new List<CaveRoom>();
			_edgeTitles   = new List<Vector2>();

		}

		public static void connectRoom(CaveRoom room1, CaveRoom room2){
			room1.connectRooms.Add(room2);
			room2.connectRooms.Add(room1);
		}
			
		public bool isConnectRoom(CaveRoom room){
			return _connectRooms.Contains (room);
		}

		public void createEdgeTiles(int [,] cMap){
			foreach(Vector2 v in _tiles){
				int left  = (int)v.x - 1;
				int right = (int)v.x + 1;
				int up 	  = (int)v.y - 1;
				int down  = (int)v.y + 1;

				for(int i = left; i <= right; i++){
					for(int j = up; j <= down; j++){
						if (v.x == i || v.y == j){
							if(cMap[i,j] == 1){
								_edgeTitles.Add(new Vector2(i,j));					
							}
						}
					}
				}
			}
		}
	}

	/*
	void connectRooms(List<CaveRoom> rooms){
		int shortDistance = 0;
		Vector2 connectPointA = Vector2.zero;
		Vector2 connectPointB = Vector2.zero;
		CaveRoom connectRoomA = new CaveRoom();
		CaveRoom connectRoomB = new CaveRoom();
		bool shouldBeConnect = false;

		foreach (CaveRoom roomA in rooms) {
			foreach (CaveRoom roomB in rooms) {
				if (roomA != roomB && !roomA.isConnectRoom(roomB)) {
					
					for (int i = 0; i < roomA.edgeTitles.Count; i++) {
						for (int j = 0; j < roomB.edgeTitles.Count; j++) {
							Vector2 tileA = roomA.edgeTitles [i];
							Vector2 tileB = roomB.edgeTitles [j];
							int distance = (int)Vector2.Distance (tileA, tileB);

							if (distance < shortDistance || !shouldBeConnect) {
								shortDistance   = distance;
								shouldBeConnect = true;
								connectPointA 	= tileA;
								connectPointB   = tileB;
								connectRoomA 	= roomA;
								connectRoomB 	= roomB;
							}
						}
					}
				}
				if (shouldBeConnect) {
					createConnect (connectRoomA, connectRoomB, connectPointA, connectPointB);
				}
			}
		}
	}


	void createConnect(CaveRoom roomA, CaveRoom roomB, Vector2 pointA, Vector2 pointB){
		List<Vector2> line = GetLine (pointA, pointB);



		foreach (Vector2 v in line) {
			//DrawCircle (v, 5);
		}
		roomA.connectRooms.Add (roomB);
		roomB.connectRooms.Add (roomA);
	}


	void DrawCircle(Vector2 c, int r) {
		for (int x = -r; x <= r; x++) {
			for (int y = -r; y <= r; y++) {
				if (x*x + y*y <= r*r) {
					int drawX = (int)c.x + x;
					int drawY = (int)c.y + y;
					if (!isOutOfBounds(drawX, drawY)) {
						caveMap[drawX,drawY] = 0;
					}
				}
			}
		}
	}

	List<Vector2> GetLine(Vector2 from, Vector2 to) {
		List<Vector2> line = new List<Vector2> ();

		int x 				= (int)from.x;
		int y 				= (int)from.y;

		int dx 				= (int)(to.x - from.x);
		int dy 				= (int)(to.y - from.y);

		bool inverted 		= false;
		int step 			= Math.Sign (dx);
		int gradientStep 	= Math.Sign (dy);

		int longest 		= Mathf.Abs (dx);
		int shortest 		= Mathf.Abs (dy);

		if (longest < shortest) {
			inverted 		= true;
			longest 		= Mathf.Abs(dy);
			shortest 		= Mathf.Abs(dx);

			step 			= Math.Sign (dy);
			gradientStep 	= Math.Sign (dx);
		}

		int gradientAccumulation = longest / 2;
		for (int i =0; i < longest; i ++) {
			line.Add(new Vector2(x,y));

			if (inverted) {
				y += step;
			}
			else {
				x += step;
			}

			gradientAccumulation += shortest;
			if (gradientAccumulation >= longest) {
				if (inverted) {
					x += gradientStep;
				}
				else {
					y += gradientStep;
				}
				gradientAccumulation -= longest;
			}
		}

		return line;
	}*/



}
