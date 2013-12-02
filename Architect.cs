using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

[RequireComponent(typeof(TerrainManager))]
public class Architect : MonoBehaviour {
	
	int cubeNumber;
	int faceNumber;
	int[,] tileTypeArray;
	TerrainManager terrainManager;
	SceneBuilder sceneBuilder;
	
	void Awake(){
		terrainManager = GetComponent<TerrainManager>();
		sceneBuilder = GetComponent<SceneBuilder>();
	}
	
	public void GenerateCube(int cubeNumber, int cubeSize, Vector3 cubeArrayPosition, bool interior){

		CubeObj thisCubeObj = new CubeObj(cubeNumber, cubeSize, interior, cubeArrayPosition);
		terrainManager.cubeMasterList.Add( thisCubeObj );
		thisCubeObj.cubeArrayPosition = cubeArrayPosition;
		Vector3 cubePosition = new Vector3(cubeArrayPosition.x * cubeSize, cubeArrayPosition.y * cubeSize, cubeArrayPosition.z * cubeSize);
		thisCubeObj.cubeGameObj.transform.parent = gameObject.transform;
		thisCubeObj.cubeGameObj.transform.position = cubePosition;
	}
	/*
	public void GenerateTerrain(CubeObj thisCube, int faceNumber){
		int cubeSize = thisCube.cubeSize;
		FaceObj startFace = thisCube.faceList[faceNumber];
		
		int roomTryCountPool = (int)Mathf.Floor(Mathf.Log(cubeSize) * 100);
		int roomCountPool = (int)Mathf.Floor(cubeSize / 2);
		
		for(int roomCount = 0; roomCount < roomCountPool; roomCount++){
			RoomObj(startFace, roomCount);

		}
		
	}
	*/

	public CubeObj[,,] NeighborCheck(Vector3 checkCoords){
		List<CubeObj> cubeMasterList = terrainManager.cubeMasterList;
		CubeObj[,,] localNeighborArray = new CubeObj[3,3,3];

		// Translate global cubePositionArray into localNeighborArray containing the cube types at the 26 coords adjacent to the parameter checkCoords
		// FirstOrDefault searches for cube at the position. If no cube is found or the coordinates are outside the level, returns default value 0

		for(int x = 0; x < localNeighborArray.GetLength(0); x++){
			for(int y = 0; y < localNeighborArray.GetLength(1); y++){
				for(int z = 0; z < localNeighborArray.GetLength(2); z++){
					CubeObj thisCube = cubeMasterList.FirstOrDefault(c => c.cubeArrayPosition == (checkCoords - new Vector3(x - 1, y - 1, z - 1)));
					if(thisCube != null){
						localNeighborArray[x,y,z] = thisCube;
					}
					else {
						localNeighborArray[x,y,z] = null;
					}
				}
			}
		}

		/* Old neighborhood code dependent upon arrays, not viable for agent locations near edge of level
		localNeighborArray[0,0,0] = cubePositionArray[(int)checkCoords.x - 1, (int)checkCoords.y - 1, (int)checkCoords.z - 1];
		localNeighborArray[1,0,0] = cubePositionArray[(int)checkCoords.x - 0, (int)checkCoords.y - 1, (int)checkCoords.z - 1];
		localNeighborArray[2,0,0] = cubePositionArray[(int)checkCoords.x + 1, (int)checkCoords.y - 1, (int)checkCoords.z - 1];
		localNeighborArray[0,1,0] = cubePositionArray[(int)checkCoords.x - 1, (int)checkCoords.y - 0, (int)checkCoords.z - 1];
		localNeighborArray[1,1,0] = cubePositionArray[(int)checkCoords.x - 0, (int)checkCoords.y - 0, (int)checkCoords.z - 1];
		localNeighborArray[2,1,0] = cubePositionArray[(int)checkCoords.x + 1, (int)checkCoords.y - 0, (int)checkCoords.z - 1];
		localNeighborArray[0,2,0] = cubePositionArray[(int)checkCoords.x - 1, (int)checkCoords.y + 1, (int)checkCoords.z - 1];
		localNeighborArray[1,2,0] = cubePositionArray[(int)checkCoords.x - 0, (int)checkCoords.y + 1, (int)checkCoords.z - 1];
		localNeighborArray[2,2,0] = cubePositionArray[(int)checkCoords.x + 1, (int)checkCoords.y + 1, (int)checkCoords.z - 1];

		localNeighborArray[0,0,1] = cubePositionArray[(int)checkCoords.x - 1, (int)checkCoords.y - 1, (int)checkCoords.z - 0];
		localNeighborArray[1,0,1] = cubePositionArray[(int)checkCoords.x - 0, (int)checkCoords.y - 1, (int)checkCoords.z - 0];
		localNeighborArray[2,0,1] = cubePositionArray[(int)checkCoords.x + 1, (int)checkCoords.y - 1, (int)checkCoords.z - 0];
		localNeighborArray[0,1,1] = cubePositionArray[(int)checkCoords.x - 1, (int)checkCoords.y - 0, (int)checkCoords.z - 0];
		localNeighborArray[1,1,1] = cubePositionArray[(int)checkCoords.x - 0, (int)checkCoords.y - 0, (int)checkCoords.z - 0];
		localNeighborArray[2,1,1] = cubePositionArray[(int)checkCoords.x + 1, (int)checkCoords.y - 0, (int)checkCoords.z - 0];
		localNeighborArray[0,2,1] = cubePositionArray[(int)checkCoords.x - 1, (int)checkCoords.y + 1, (int)checkCoords.z - 0];
		localNeighborArray[1,2,1] = cubePositionArray[(int)checkCoords.x - 0, (int)checkCoords.y + 1, (int)checkCoords.z - 0];
		localNeighborArray[2,2,1] = cubePositionArray[(int)checkCoords.x + 1, (int)checkCoords.y + 1, (int)checkCoords.z - 0];

		localNeighborArray[0,0,2] = cubePositionArray[(int)checkCoords.x - 1, (int)checkCoords.y - 1, (int)checkCoords.z + 1];
		localNeighborArray[1,0,2] = cubePositionArray[(int)checkCoords.x - 0, (int)checkCoords.y - 1, (int)checkCoords.z + 1];
		localNeighborArray[2,0,2] = cubePositionArray[(int)checkCoords.x + 1, (int)checkCoords.y - 1, (int)checkCoords.z + 1];
		localNeighborArray[0,1,2] = cubePositionArray[(int)checkCoords.x - 1, (int)checkCoords.y - 0, (int)checkCoords.z + 1];
		localNeighborArray[1,1,2] = cubePositionArray[(int)checkCoords.x - 0, (int)checkCoords.y - 0, (int)checkCoords.z + 1];
		localNeighborArray[2,1,2] = cubePositionArray[(int)checkCoords.x + 1, (int)checkCoords.y - 0, (int)checkCoords.z + 1];
		localNeighborArray[0,2,2] = cubePositionArray[(int)checkCoords.x - 1, (int)checkCoords.y + 1, (int)checkCoords.z + 1];
		localNeighborArray[1,2,2] = cubePositionArray[(int)checkCoords.x - 0, (int)checkCoords.y + 1, (int)checkCoords.z + 1];
		localNeighborArray[2,2,2] = cubePositionArray[(int)checkCoords.x + 1, (int)checkCoords.y + 1, (int)checkCoords.z + 1];
		*/

		return localNeighborArray;
	}

	public int CountNeighbors(CubeObj[,,] neighborhood){

		int neighborCount = 0;

		for(int x = 0; x < neighborhood.GetLength(0); x++){
			for(int y = 0; y < neighborhood.GetLength(1); y++){
				for(int z = 0; z < neighborhood.GetLength(2); z++){
					if(neighborhood[x,y,z] != null && (x != 1 && y != 1 && z != 1)){
						neighborCount++;
					}
				}
			}
		}

		return neighborCount;
	}

	public void GenerateTextures(CubeObj cube){
		foreach(FaceObj face in cube.faceList){
			int texSize = face.faceSize * terrainManager.tileResolution;
			face.faceTexture = new Texture2D(texSize,texSize);
			int tileResolution = terrainManager.tileResolution;
			
			for (int y = 0; y < face.faceSize; y++){
				for(int x = 0; x < face.faceSize; x++){
					Color[] pixels = terrainManager.tileAtlas.GetPixels (face.tileTypeArray[x,y] * tileResolution, 0, tileResolution, tileResolution);
					face.faceTexture.SetPixels(x * tileResolution, y * tileResolution, tileResolution, tileResolution, pixels);
				}
			}
			
			face.faceTexture.filterMode = FilterMode.Point;
			face.faceTexture.wrapMode = TextureWrapMode.Clamp;
			face.faceTexture.Apply();
			face.faceMeshRenderer.material.mainTexture = face.faceTexture;
		
		}
	}
	
	public static int TileRead(int xCoord, int yCoord, FaceObj originFace){
			
		/*
		For parameter xCoord and yCoord:
		Assume that the origin face's pivot acts as a global 0,0 origin across a 2D plane formed by the origin face and its adjacent faces
		E.g. if a requested coordinate is "beneath" the origin face, then the yCoord should be passed to TileRead() as a negative
		
		NOTE: The xCoord and yCoord are zero-indexed
		
		*/
		int faceSize = originFace.faceSize;
		int faceNumber = originFace.faceNumber;
		CubeObj onCube = originFace.onCube;
		int edgeNumber = GetEdgeNumber(xCoord, yCoord, faceSize);
		int faceZeroIndexSize = faceSize - 1;
		
		// If xCoord and yCoord both exceed the faceSize, then this tile is eliminated when the faces are folded to form a cube and should return null
		
		if (xCoord >= faceSize && yCoord >= faceSize){
			return 0;
		}
		
		// If the xCoord and yCoord are within the face's bounds, just return the tileTypeArray for the parameter coordinates
		
		else if(xCoord < faceSize && yCoord < faceSize){
			return originFace.tileTypeArray[xCoord, yCoord];	
		}
		
		// If the coordinates are neither within the origin face's bounds nor outside the x and y dimension bounds of the face, then they must be on an adjacent face
		
		else{
			if(faceNumber == 0){
				switch(edgeNumber){
				case 0:
					Debug.Log ("xCoord: " +xCoord+ " yCoord: " +yCoord);
					Debug.Log ("faceSize: " +faceSize);
					Debug.Log ("Requesting read at X: " +(xCoord)+ " Y: " + (yCoord - faceSize));
					Debug.Log ("Array size is X: " +onCube.faceList[4].tileTypeArray.GetLength(0)+ " Y: " +onCube.faceList[4].tileTypeArray.GetLength(1));
					return onCube.faceList[4].tileTypeArray[xCoord, yCoord - faceSize];
				case 1:
					Debug.Log ("xCoord: " +xCoord+ " yCoord: " +yCoord);
					Debug.Log ("faceSize: " +faceSize);
					Debug.Log ("Requesting read at X: " +(xCoord - faceSize)+ " Y: " + (yCoord));
					Debug.Log ("Array size is X: " +onCube.faceList[1].tileTypeArray.GetLength(0)+ " Y: " +onCube.faceList[1].tileTypeArray.GetLength(1));
					return onCube.faceList[1].tileTypeArray[xCoord - faceSize, yCoord];
				case 2:
					Debug.Log ("xCoord: " +xCoord+ " yCoord: " +yCoord);
					Debug.Log ("faceSize: " +faceSize);
					Debug.Log ("Requesting read at X: " +(xCoord)+ " Y: " + (faceSize + yCoord));
					Debug.Log ("Array size is X: " +onCube.faceList[2].tileTypeArray.GetLength(0)+ " Y: " +onCube.faceList[2].tileTypeArray.GetLength(1));
					return onCube.faceList[2].tileTypeArray[xCoord, faceSize + yCoord];
				case 3:
					Debug.Log ("xCoord: " +xCoord+ " yCoord: " +yCoord);
					Debug.Log ("faceSize: " +faceSize);
					Debug.Log ("Requesting read at X: " +(faceSize + xCoord)+ " Y: " + (yCoord));
					Debug.Log ("Array size is X: " +onCube.faceList[3].tileTypeArray.GetLength(0)+ " Y: " +onCube.faceList[3].tileTypeArray.GetLength(1));
					return onCube.faceList[3].tileTypeArray[faceSize + xCoord, yCoord];
				}
			}
			
			else if(faceNumber == 1){
				switch(edgeNumber){
				case 0:
					Debug.Log ("xCoord: " +xCoord+ " yCoord: " +yCoord);
					Debug.Log ("faceSize: " +faceSize);
					Debug.Log ("Requesting read at X: " +(faceSize - (yCoord - faceZeroIndexSize))+ " Y: " + (xCoord));
					Debug.Log ("Array size is X: " +onCube.faceList[4].tileTypeArray.GetLength(0)+ " Y: " +onCube.faceList[4].tileTypeArray.GetLength(1));
					return onCube.faceList[4].tileTypeArray[faceSize - (yCoord - faceZeroIndexSize), xCoord];
				case 1:
					Debug.Log ("xCoord: " +xCoord+ " yCoord: " +yCoord);
					Debug.Log ("faceSize: " +faceSize);
					Debug.Log ("Requesting read at X: " +(faceSize - (xCoord - faceZeroIndexSize))+ " Y: " + (faceZeroIndexSize - yCoord));
					Debug.Log ("Array size is X: " +onCube.faceList[5].tileTypeArray.GetLength(0)+ " Y: " +onCube.faceList[5].tileTypeArray.GetLength(1));
					return onCube.faceList[5].tileTypeArray[faceSize - (xCoord - faceZeroIndexSize), faceZeroIndexSize - yCoord];
				case 2:
					Debug.Log ("xCoord: " +xCoord+ " yCoord: " +yCoord);
					Debug.Log ("faceSize: " +faceSize);
					Debug.Log ("Requesting read at X: " +(faceSize + yCoord)+ " Y: " + (faceZeroIndexSize - xCoord));
					Debug.Log ("Array size is X: " +onCube.faceList[2].tileTypeArray.GetLength(0)+ " Y: " +onCube.faceList[2].tileTypeArray.GetLength(1));
					return onCube.faceList[2].tileTypeArray[faceSize + yCoord, faceZeroIndexSize - xCoord];
				case 3:
					Debug.Log ("xCoord: " +xCoord+ " yCoord: " +yCoord);
					Debug.Log ("faceSize: " +faceSize);
					Debug.Log ("Requesting read at X: " +(faceSize + xCoord)+ " Y: " + (yCoord));
					Debug.Log ("Array size is X: " +onCube.faceList[0].tileTypeArray.GetLength(0)+ " Y: " +onCube.faceList[0].tileTypeArray.GetLength(1));
					return onCube.faceList[0].tileTypeArray[faceSize + xCoord, yCoord];
				}
			}
			
			else if(faceNumber == 2){
				switch(edgeNumber){
				case 0:
					Debug.Log ("xCoord: " +xCoord+ " yCoord: " +yCoord);
					Debug.Log ("faceSize: " +faceSize);
					Debug.Log ("Requesting read at X: " +(xCoord)+ " Y: " + (yCoord - faceSize));
					Debug.Log ("Array size is X: " +onCube.faceList[0].tileTypeArray.GetLength(0)+ " Y: " +onCube.faceList[0].tileTypeArray.GetLength(1));
					return onCube.faceList[0].tileTypeArray[xCoord, yCoord - faceSize];
				case 1:
					Debug.Log ("xCoord: " +xCoord+ " yCoord: " +yCoord);
					Debug.Log ("faceSize: " +faceSize);
					Debug.Log ("Requesting read at X: " +(faceZeroIndexSize - yCoord)+ " Y: " + (xCoord - faceSize));
					Debug.Log ("Array size is X: " +onCube.faceList[1].tileTypeArray.GetLength(0)+ " Y: " +onCube.faceList[1].tileTypeArray.GetLength(1));
					return onCube.faceList[1].tileTypeArray[faceZeroIndexSize - yCoord, xCoord - faceSize];
				case 2:
					Debug.Log ("xCoord: " +xCoord+ " yCoord: " +yCoord);
					Debug.Log ("faceSize: " +faceSize);
					Debug.Log ("Requesting read at X: " +(xCoord)+ " Y: " + (faceSize + yCoord));
					Debug.Log ("Array size is X: " +onCube.faceList[5].tileTypeArray.GetLength(0)+ " Y: " +onCube.faceList[5].tileTypeArray.GetLength(1));
					return onCube.faceList[5].tileTypeArray[xCoord, faceSize + yCoord];
				case 3:
					Debug.Log ("xCoord: " +xCoord+ " yCoord: " +yCoord);
					Debug.Log ("faceSize: " +faceSize);
					Debug.Log ("Requesting read at X: " +(yCoord)+ " Y: " + (-xCoord - 1));
					Debug.Log ("Array size is X: " +onCube.faceList[3].tileTypeArray.GetLength(0)+ " Y: " +onCube.faceList[3].tileTypeArray.GetLength(1));
					return onCube.faceList[3].tileTypeArray[yCoord, -xCoord - 1];
				}
			}
			
			else if(faceNumber == 3){
				switch(edgeNumber){
				case 0:
					Debug.Log ("xCoord: " +xCoord+ " yCoord: " +yCoord);
					Debug.Log ("faceSize: " +faceSize);
					Debug.Log ("Requesting read at X: " +(yCoord - faceSize)+ " Y: " + (faceZeroIndexSize - xCoord));
					Debug.Log ("Array size is X: " +onCube.faceList[4].tileTypeArray.GetLength(0)+ " Y: " +onCube.faceList[4].tileTypeArray.GetLength(1));
					return onCube.faceList[4].tileTypeArray[yCoord - faceSize, faceZeroIndexSize - xCoord];
				case 1:
					Debug.Log ("xCoord: " +xCoord+ " yCoord: " +yCoord);
					Debug.Log ("faceSize: " +faceSize);
					Debug.Log ("Requesting read at X: " +(xCoord - faceSize)+ " Y: " + (yCoord));
					Debug.Log ("Array size is X: " +onCube.faceList[0].tileTypeArray.GetLength(0)+ " Y: " +onCube.faceList[0].tileTypeArray.GetLength(1));
					return onCube.faceList[0].tileTypeArray[xCoord - faceSize, yCoord];
				case 2:
					Debug.Log ("xCoord: " +xCoord+ " yCoord: " +yCoord);
					Debug.Log ("faceSize: " +faceSize);
					Debug.Log ("Requesting read at X: " +(-yCoord - 1)+ " Y: " + (xCoord));
					Debug.Log ("Array size is X: " +onCube.faceList[2].tileTypeArray.GetLength(0)+ " Y: " +onCube.faceList[2].tileTypeArray.GetLength(1));
					return onCube.faceList[2].tileTypeArray[-yCoord - 1, xCoord];
				case 3:
					Debug.Log ("xCoord: " +xCoord+ " yCoord: " +yCoord);
					Debug.Log ("faceSize: " +faceSize);
					Debug.Log ("Requesting read at X: " +(-xCoord - 1)+ " Y: " + (faceZeroIndexSize - yCoord));
					Debug.Log ("Array size is X: " +onCube.faceList[5].tileTypeArray.GetLength(0)+ " Y: " +onCube.faceList[5].tileTypeArray.GetLength(1));
					return onCube.faceList[5].tileTypeArray[-xCoord - 1, faceZeroIndexSize - yCoord];
				}
			}
			
			else if(faceNumber == 4){
				switch(edgeNumber){
				case 0:
					Debug.Log ("xCoord: " +xCoord+ " yCoord: " +yCoord);
					Debug.Log ("faceSize: " +faceSize);
					Debug.Log ("Requesting read at X: " +(xCoord)+ " Y: " + (yCoord - faceSize));
					Debug.Log ("Array size is X: " +onCube.faceList[5].tileTypeArray.GetLength(0)+ " Y: " +onCube.faceList[5].tileTypeArray.GetLength(1));
					return onCube.faceList[5].tileTypeArray[xCoord, yCoord - faceSize];
				case 1:
					Debug.Log ("xCoord: " +xCoord+ " yCoord: " +yCoord);
					Debug.Log ("faceSize: " +faceSize);
					Debug.Log ("Requesting read at X: " +(yCoord)+ " Y: " + (faceSize - (xCoord - faceZeroIndexSize)));
					Debug.Log ("Array size is X: " +onCube.faceList[1].tileTypeArray.GetLength(0)+ " Y: " +onCube.faceList[1].tileTypeArray.GetLength(1));
					return onCube.faceList[1].tileTypeArray[yCoord, faceSize - (xCoord - faceZeroIndexSize)];
				case 2:
					Debug.Log ("xCoord: " +xCoord+ " yCoord: " +yCoord);
					Debug.Log ("faceSize: " +faceSize);
					Debug.Log ("Requesting read at X: " +(xCoord)+ " Y: " + (faceSize + yCoord));
					Debug.Log ("Array size is X: " +onCube.faceList[0].tileTypeArray.GetLength(0)+ " Y: " +onCube.faceList[0].tileTypeArray.GetLength(1));
					return onCube.faceList[0].tileTypeArray[xCoord, faceSize + yCoord];
				case 3:
					Debug.Log ("xCoord: " +xCoord+ " yCoord: " +yCoord);
					Debug.Log ("faceSize: " +faceSize);
					Debug.Log ("Requesting read at X: " +(faceZeroIndexSize - yCoord)+ " Y: " + (faceSize + xCoord));
					Debug.Log ("Array size is X: " +onCube.faceList[3].tileTypeArray.GetLength(0)+ " Y: " +onCube.faceList[3].tileTypeArray.GetLength(1));
					return onCube.faceList[3].tileTypeArray[faceZeroIndexSize - yCoord, faceSize + xCoord];
				}
			}
			
			else if(faceNumber == 5){
				switch(edgeNumber){
				case 0:
					Debug.Log ("xCoord: " +xCoord+ " yCoord: " +yCoord);
					Debug.Log ("faceSize: " +faceSize);
					Debug.Log ("Requesting read at X: " +(xCoord)+ " Y: " + (yCoord - faceSize));
					Debug.Log ("Array size is X: " +onCube.faceList[2].tileTypeArray.GetLength(0)+ " Y: " +onCube.faceList[2].tileTypeArray.GetLength(1));
					return onCube.faceList[2].tileTypeArray[xCoord, yCoord - faceSize];
				case 1:
					Debug.Log ("xCoord: " +xCoord+ " yCoord: " +yCoord);
					Debug.Log ("faceSize: " +faceSize);
					Debug.Log ("Requesting read at X: " +(faceSize - (xCoord - faceZeroIndexSize))+ " Y: " + (faceZeroIndexSize - yCoord));
					Debug.Log ("Array size is X: " +onCube.faceList[1].tileTypeArray.GetLength(0)+ " Y: " +onCube.faceList[1].tileTypeArray.GetLength(1));
					return onCube.faceList[1].tileTypeArray[faceSize - (xCoord - faceZeroIndexSize), faceZeroIndexSize - yCoord];
				case 2:
					Debug.Log ("xCoord: " +xCoord+ " yCoord: " +yCoord);
					Debug.Log ("faceSize: " +faceSize);
					Debug.Log ("Requesting read at X: " +(xCoord)+ " Y: " + (faceSize + yCoord));
					Debug.Log ("Array size is X: " +onCube.faceList[4].tileTypeArray.GetLength(0)+ " Y: " +onCube.faceList[4].tileTypeArray.GetLength(1));
					return onCube.faceList[4].tileTypeArray[xCoord, faceSize + yCoord];
				case 3:
					Debug.Log ("xCoord: " +xCoord+ " yCoord: " +yCoord);
					Debug.Log ("faceSize: " +faceSize);
					Debug.Log ("Requesting read at X: " +(-xCoord - 1)+ " Y: " + (faceZeroIndexSize - yCoord));
					Debug.Log ("Array size is X: " +onCube.faceList[3].tileTypeArray.GetLength(0)+ " Y: " +onCube.faceList[3].tileTypeArray.GetLength(1));
					return onCube.faceList[3].tileTypeArray[-xCoord - 1, faceZeroIndexSize - yCoord];
				}
			}
			return 0;	
		}
	}
		
	public static bool TileWrite(int xCoord, int yCoord, int tileType, FaceObj originFace){
		
		/*
		For parameter xCoord and yCoord:
		Assume that the origin face's pivot acts as a global 0,0 origin across a 2D plane formed by the origin face and its adjacent faces
		E.g. if a requested coordinate is "beneath" the origin face, then the yCoord should be passed to TileWrite() as a negative
		
		Return true if the write was successful and will appear in a cube face's tileTypeArray. If the coordinate will be lost in a fold, return false.
		
		NOTE: The xCoord and yCoord are zero-indexed
		
		*/
		
		// If xCoord and yCoord both exceed the faceSize, then this tile is eliminated when the faces are folded to form a cube and should return false
		
		int faceSize = originFace.faceSize;
		int faceNumber = originFace.faceNumber;
		CubeObj onCube = originFace.onCube;
		int faceZeroIndexSize = faceSize - 1;
		
		if (xCoord >= faceSize && yCoord >= faceSize){
			Debug.Log ("In the null fold zone");
			return false;	
		}
		
		else if(xCoord < faceSize && yCoord < faceSize){
			originFace.tileTypeArray[xCoord,yCoord] = tileType;
			Debug.Log ("On the proper face");
			return true;
		}
		
		else{
		
			// If the specified tile is a valid post-fold coordinate, then set that tile and return true
		
			int edgeNumber = GetEdgeNumber(xCoord, yCoord, faceSize);
			
			if(faceNumber == 0){
				switch(edgeNumber){
				case 0:
					Debug.Log ("xCoord: " +xCoord+ " yCoord: " +yCoord);
					Debug.Log ("faceSize: " +faceSize);
					Debug.Log ("Requesting read at X: " +(xCoord)+ " Y: " + (yCoord - faceSize));
					Debug.Log ("Array size is X: " +onCube.faceList[4].tileTypeArray.GetLength(0)+ " Y: " +onCube.faceList[4].tileTypeArray.GetLength(1));
					onCube.faceList[4].tileTypeArray[xCoord, yCoord - faceSize] = tileType;
					return true;
				case 1:
					Debug.Log ("xCoord: " +xCoord+ " yCoord: " +yCoord);
					Debug.Log ("faceSize: " +faceSize);
					Debug.Log ("Requesting read at X: " +(xCoord - faceSize)+ " Y: " + (yCoord));
					Debug.Log ("Array size is X: " +onCube.faceList[1].tileTypeArray.GetLength(0)+ " Y: " +onCube.faceList[1].tileTypeArray.GetLength(1));
					onCube.faceList[1].tileTypeArray[xCoord - faceSize, yCoord] = tileType;
					return true;
				case 2:
					Debug.Log ("xCoord: " +xCoord+ " yCoord: " +yCoord);
					Debug.Log ("faceSize: " +faceSize);
					Debug.Log ("Requesting read at X: " +(xCoord)+ " Y: " + (faceSize + yCoord));
					Debug.Log ("Array size is X: " +onCube.faceList[2].tileTypeArray.GetLength(0)+ " Y: " +onCube.faceList[2].tileTypeArray.GetLength(1));
					onCube.faceList[2].tileTypeArray[xCoord, faceSize + yCoord] = tileType;
					return true;
				case 3:
					Debug.Log ("xCoord: " +xCoord+ " yCoord: " +yCoord);
					Debug.Log ("faceSize: " +faceSize);
					Debug.Log ("Requesting read at X: " +(faceSize + xCoord)+ " Y: " + (yCoord));
					Debug.Log ("Array size is X: " +onCube.faceList[3].tileTypeArray.GetLength(0)+ " Y: " +onCube.faceList[3].tileTypeArray.GetLength(1));
					onCube.faceList[3].tileTypeArray[faceSize + xCoord, yCoord] = tileType;
					return true;
				}
			}
			
			if(faceNumber == 1){
				switch(edgeNumber){
				case 0:
					Debug.Log ("xCoord: " +xCoord+ " yCoord: " +yCoord);
					Debug.Log ("faceSize: " +faceSize);
					Debug.Log ("Requesting read at X: " +(faceSize - (yCoord - faceZeroIndexSize))+ " Y: " + (xCoord));
					Debug.Log ("Array size is X: " +onCube.faceList[4].tileTypeArray.GetLength(0)+ " Y: " +onCube.faceList[4].tileTypeArray.GetLength(1));
					onCube.faceList[4].tileTypeArray[faceSize - (yCoord - faceZeroIndexSize), xCoord] = tileType;
					return true;
				case 1:
					Debug.Log ("xCoord: " +xCoord+ " yCoord: " +yCoord);
					Debug.Log ("faceSize: " +faceSize);
					Debug.Log ("Requesting read at X: " +(faceSize - (xCoord - faceZeroIndexSize))+ " Y: " + (faceZeroIndexSize - yCoord));
					Debug.Log ("Array size is X: " +onCube.faceList[5].tileTypeArray.GetLength(0)+ " Y: " +onCube.faceList[5].tileTypeArray.GetLength(1));
					onCube.faceList[5].tileTypeArray[faceSize - (xCoord - faceZeroIndexSize), faceZeroIndexSize - yCoord] = tileType;
					return true;
				case 2:
					Debug.Log ("xCoord: " +xCoord+ " yCoord: " +yCoord);
					Debug.Log ("faceSize: " +faceSize);
					Debug.Log ("Requesting read at X: " +(faceSize + yCoord)+ " Y: " + (faceZeroIndexSize - xCoord));
					Debug.Log ("Array size is X: " +onCube.faceList[2].tileTypeArray.GetLength(0)+ " Y: " +onCube.faceList[2].tileTypeArray.GetLength(1));
					onCube.faceList[2].tileTypeArray[faceSize + yCoord, faceZeroIndexSize - xCoord] = tileType;
					return true;
				case 3:
					Debug.Log ("xCoord: " +xCoord+ " yCoord: " +yCoord);
					Debug.Log ("faceSize: " +faceSize);
					Debug.Log ("Requesting read at X: " +(faceSize + xCoord)+ " Y: " + (yCoord));
					Debug.Log ("Array size is X: " +onCube.faceList[0].tileTypeArray.GetLength(0)+ " Y: " +onCube.faceList[0].tileTypeArray.GetLength(1));
					onCube.faceList[0].tileTypeArray[faceSize + xCoord, yCoord] = tileType;
					return true;
				}
			}
			
			if(faceNumber == 2){
				switch(edgeNumber){
				case 0:
					Debug.Log ("xCoord: " +xCoord+ " yCoord: " +yCoord);
					Debug.Log ("faceSize: " +faceSize);
					Debug.Log ("Requesting read at X: " +(xCoord)+ " Y: " + (yCoord - faceSize));
					Debug.Log ("Array size is X: " +onCube.faceList[0].tileTypeArray.GetLength(0)+ " Y: " +onCube.faceList[0].tileTypeArray.GetLength(1));
					onCube.faceList[0].tileTypeArray[xCoord, yCoord - faceSize] = tileType;
					return true;
				case 1:
					Debug.Log ("xCoord: " +xCoord+ " yCoord: " +yCoord);
					Debug.Log ("faceSize: " +faceSize);
					Debug.Log ("Requesting read at X: " +(faceZeroIndexSize - yCoord)+ " Y: " + (xCoord - faceSize));
					Debug.Log ("Array size is X: " +onCube.faceList[1].tileTypeArray.GetLength(0)+ " Y: " +onCube.faceList[1].tileTypeArray.GetLength(1));
					onCube.faceList[1].tileTypeArray[faceZeroIndexSize - yCoord, xCoord - faceSize] = tileType;
					return true;
				case 2:
					Debug.Log ("xCoord: " +xCoord+ " yCoord: " +yCoord);
					Debug.Log ("faceSize: " +faceSize);
					Debug.Log ("Requesting read at X: " +(xCoord)+ " Y: " + (faceSize + yCoord));
					Debug.Log ("Array size is X: " +onCube.faceList[5].tileTypeArray.GetLength(0)+ " Y: " +onCube.faceList[5].tileTypeArray.GetLength(1));
					onCube.faceList[5].tileTypeArray[xCoord, faceSize + yCoord] = tileType;
					return true;
				case 3:
					Debug.Log ("xCoord: " +xCoord+ " yCoord: " +yCoord);
					Debug.Log ("faceSize: " +faceSize);
					Debug.Log ("Requesting read at X: " +(yCoord)+ " Y: " + (-xCoord - 1));
					Debug.Log ("Array size is X: " +onCube.faceList[3].tileTypeArray.GetLength(0)+ " Y: " +onCube.faceList[3].tileTypeArray.GetLength(1));
					onCube.faceList[3].tileTypeArray[yCoord, -xCoord - 1] = tileType;
					return true;
				}
			}
			
			if(faceNumber == 3){
				switch(edgeNumber){
				case 0:
					Debug.Log ("xCoord: " +xCoord+ " yCoord: " +yCoord);
					Debug.Log ("faceSize: " +faceSize);
					Debug.Log ("Requesting read at X: " +(yCoord - faceSize)+ " Y: " + (faceZeroIndexSize - xCoord));
					Debug.Log ("Array size is X: " +onCube.faceList[4].tileTypeArray.GetLength(0)+ " Y: " +onCube.faceList[4].tileTypeArray.GetLength(1));
					onCube.faceList[4].tileTypeArray[yCoord - faceSize, faceZeroIndexSize - xCoord] = tileType;
					return true;
				case 1:
					Debug.Log ("xCoord: " +xCoord+ " yCoord: " +yCoord);
					Debug.Log ("faceSize: " +faceSize);
					Debug.Log ("Requesting read at X: " +(xCoord - faceSize)+ " Y: " + (yCoord));
					Debug.Log ("Array size is X: " +onCube.faceList[0].tileTypeArray.GetLength(0)+ " Y: " +onCube.faceList[0].tileTypeArray.GetLength(1));
					onCube.faceList[0].tileTypeArray[xCoord - faceSize, yCoord] = tileType;
					return true;
				case 2:
					Debug.Log ("xCoord: " +xCoord+ " yCoord: " +yCoord);
					Debug.Log ("faceSize: " +faceSize);
					Debug.Log ("Requesting read at X: " +(-yCoord - 1)+ " Y: " + (xCoord));
					Debug.Log ("Array size is X: " +onCube.faceList[2].tileTypeArray.GetLength(0)+ " Y: " +onCube.faceList[2].tileTypeArray.GetLength(1));
					onCube.faceList[2].tileTypeArray[-yCoord - 1, xCoord] = tileType;
					return true;
				case 3:
					Debug.Log ("xCoord: " +xCoord+ " yCoord: " +yCoord);
					Debug.Log ("faceSize: " +faceSize);
					Debug.Log ("Requesting read at X: " +(-xCoord - 1)+ " Y: " + (faceZeroIndexSize - yCoord));
					Debug.Log ("Array size is X: " +onCube.faceList[5].tileTypeArray.GetLength(0)+ " Y: " +onCube.faceList[5].tileTypeArray.GetLength(1));
					onCube.faceList[5].tileTypeArray[-xCoord - 1, faceZeroIndexSize - yCoord] = tileType;
					return true;
				}
			}
			
			if(faceNumber == 4){
				switch(edgeNumber){
				case 0:
					Debug.Log ("xCoord: " +xCoord+ " yCoord: " +yCoord);
					Debug.Log ("faceSize: " +faceSize);
					Debug.Log ("Requesting read at X: " +(xCoord)+ " Y: " + (yCoord - faceSize));
					Debug.Log ("Array size is X: " +onCube.faceList[5].tileTypeArray.GetLength(0)+ " Y: " +onCube.faceList[5].tileTypeArray.GetLength(1));
					onCube.faceList[5].tileTypeArray[xCoord, yCoord - faceSize] = tileType;
					return true;
				case 1:
					Debug.Log ("xCoord: " +xCoord+ " yCoord: " +yCoord);
					Debug.Log ("faceSize: " +faceSize);
					Debug.Log ("Requesting read at X: " +(yCoord)+ " Y: " + (faceSize - (xCoord - faceZeroIndexSize)));
					Debug.Log ("Array size is X: " +onCube.faceList[1].tileTypeArray.GetLength(0)+ " Y: " +onCube.faceList[1].tileTypeArray.GetLength(1));
					onCube.faceList[1].tileTypeArray[yCoord, faceSize - (xCoord - faceZeroIndexSize)] = tileType;
					return true;
				case 2:
					Debug.Log ("xCoord: " +xCoord+ " yCoord: " +yCoord);
					Debug.Log ("faceSize: " +faceSize);
					Debug.Log ("Requesting read at X: " +(xCoord)+ " Y: " + (faceSize + yCoord));
					Debug.Log ("Array size is X: " +onCube.faceList[0].tileTypeArray.GetLength(0)+ " Y: " +onCube.faceList[0].tileTypeArray.GetLength(1));
					onCube.faceList[0].tileTypeArray[xCoord, faceSize + yCoord] = tileType;
					return true;
				case 3:
					Debug.Log ("xCoord: " +xCoord+ " yCoord: " +yCoord);
					Debug.Log ("faceSize: " +faceSize);
					Debug.Log ("Requesting read at X: " +(faceZeroIndexSize - yCoord)+ " Y: " + (faceSize + xCoord));
					Debug.Log ("Array size is X: " +onCube.faceList[3].tileTypeArray.GetLength(0)+ " Y: " +onCube.faceList[3].tileTypeArray.GetLength(1));
					onCube.faceList[3].tileTypeArray[faceZeroIndexSize - yCoord, faceSize + xCoord] = tileType;
					return true;
				}
			}
			
			if(faceNumber == 5){
				switch(edgeNumber){
				case 0:
					Debug.Log ("xCoord: " +xCoord+ " yCoord: " +yCoord);
					Debug.Log ("faceSize: " +faceSize);
					Debug.Log ("Requesting read at X: " +(xCoord)+ " Y: " + (yCoord - faceSize));
					Debug.Log ("Array size is X: " +onCube.faceList[2].tileTypeArray.GetLength(0)+ " Y: " +onCube.faceList[2].tileTypeArray.GetLength(1));
					onCube.faceList[2].tileTypeArray[xCoord, yCoord - faceSize] = tileType;
					return true;
				case 1:
					Debug.Log ("xCoord: " +xCoord+ " yCoord: " +yCoord);
					Debug.Log ("faceSize: " +faceSize);
					Debug.Log ("Requesting read at X: " +(faceSize - (xCoord - faceZeroIndexSize))+ " Y: " + (faceZeroIndexSize - yCoord));
					Debug.Log ("Array size is X: " +onCube.faceList[1].tileTypeArray.GetLength(0)+ " Y: " +onCube.faceList[1].tileTypeArray.GetLength(1));
					onCube.faceList[1].tileTypeArray[faceSize - (xCoord - faceZeroIndexSize), faceZeroIndexSize - yCoord] = tileType;
					return true;
				case 2:
					Debug.Log ("xCoord: " +xCoord+ " yCoord: " +yCoord);
					Debug.Log ("faceSize: " +faceSize);
					Debug.Log ("Requesting read at X: " +(xCoord)+ " Y: " + (faceSize + yCoord));
					Debug.Log ("Array size is X: " +onCube.faceList[4].tileTypeArray.GetLength(0)+ " Y: " +onCube.faceList[4].tileTypeArray.GetLength(1));
					onCube.faceList[4].tileTypeArray[xCoord, faceSize + yCoord] = tileType;
					return true;
				case 3:
					Debug.Log ("xCoord: " +xCoord+ " yCoord: " +yCoord);
					Debug.Log ("faceSize: " +faceSize);
					Debug.Log ("Requesting read at X: " +(-xCoord - 1)+ " Y: " + (faceZeroIndexSize - yCoord));
					Debug.Log ("Array size is X: " +onCube.faceList[3].tileTypeArray.GetLength(0)+ " Y: " +onCube.faceList[3].tileTypeArray.GetLength(1));
					onCube.faceList[3].tileTypeArray[-xCoord - 1, faceZeroIndexSize - yCoord] = tileType;
					return true;
				}
			}
			Debug.Log ("fell into a black hole!");
			return false;
		}
	}
		
	public static int GetEdgeNumber(int xCoord, int yCoord, int faceSize){
		
		if(yCoord >= faceSize && xCoord < faceSize){
			return 0;	
		}
		
		else if (xCoord >= faceSize && yCoord < faceSize){
			return 1;
		}
		
		else if (xCoord < faceSize && yCoord < 0){
			return 2;
		}
		
		else if (xCoord < 0 && yCoord < faceSize){
			return 3;
		}
	 	return 0;	
	}
}

