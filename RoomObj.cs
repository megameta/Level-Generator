using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class RoomObj {

	public int cubeNumber;
	public int faceNumber;
	public int faceSize;
	public int xStart;
	public int yStart;
	public int xSize;
	public int ySize;
	public int roomType;
	public int roomNumber;
	public int[,] roomTileArray;
	public bool overlap;
	public bool createSuccess;
	
	public List<RoomObj> connectRoomList = new List<RoomObj>();
	
	FaceObj onFace;
	
	/*
	
	//Rotation holds values for the times by which the room is to be rotated (by ninety degrees clockwise) on its face
	
	public enum Rotation {
		ZERO,
		ONE,
		TWO,
		THREE
	}
	
	*/
	
	List<int[,]> roomSizeList = new List<int[,]>() {
		{new int[4,4]},
		{new int[4,8]},
		{new int[8,8]},
		{new int[16,16]},
		{new int[12, 16]}
	};
	
	public RoomObj(CubeObj onCube, FaceObj originFace, int roomCount){
		onFace = originFace;
		this.cubeNumber = onFace.cubeNumber;
		this.faceNumber = onFace.faceNumber;
		this.faceSize = onFace.faceSize;

		if(roomCount == 0){
			xSize = Random.Range(5, faceSize - 2);
			ySize = Random.Range(5, faceSize - 2);
			xStart = (int)Mathf.Floor((faceSize - xSize) / 2);
			yStart = (int)Mathf.Floor((faceSize - xSize) / 2);
		}
		else{
			xStart = Random.Range(0, faceSize);
			yStart = Random.Range(0, faceSize);
			roomType = Random.Range(0, roomSizeList.Count);
		}
		
		roomTileArray = roomSizeList[roomType];
		xSize = roomTileArray.GetLength(0);
		ySize = roomTileArray.GetLength(1);
		
		for(int y = 0; y < ySize; y++){
			for(int x = 0; x < xSize; x++){
				if (x == xSize - 1 || y == ySize - 1 || x == 0 || y == 0){
					roomTileArray[x,y] = TileObj.TileDict["wall"];
				}
				else{
					roomTileArray[x,y] = TileObj.TileDict["floor"];	
				}
			}
		}
		
		createSuccess = true;
		
	}
	
	/*	When checking for overlap, edges of a face are associated with the these numbers:
	 * 
	 * 			0
	 * 		__________
	 * 		|		  |
	 * 	3	|		  |   1
	 * 		|		  |
	 * 		|_________|
	 * 
	 * 			2
	 * 
	 */
	
	
	public void CheckOverlap(FaceObj thisFace){
		Debug.Log ("Face number: " +thisFace.faceNumber+ " Room number: " +thisFace.roomList.Count+ " Now beginning CheckOverlap");
		Debug.Log ("xStart: " +xStart+ " yStart: " +yStart);
		Debug.Log ("xSize: " +xSize+ " ySize: " +ySize);
		for(int y = yStart; y < yStart + ySize; y++){
			for(int x = xStart; x < xStart + xSize; x++){
				Debug.Log ("X: " +x+ " Y: " +y);
				
				Debug.Log ("TileRead for faceNumber: " +faceNumber+ " Room Number: "+roomNumber);
				int thisTile = Architect.TileRead(x,y, thisFace);
				if(thisTile == 0) {
					Debug.Log ("Not overlapping");
					overlap = false;
				}
				else{
					overlap = true;
					Debug.Log ("-----------------------Overlap, discard--------------------------------");
					break;
				}
			}
		}		
	}
	
	public void DrawRoom(FaceObj thisFace){
		for(int y = yStart; y < yStart + ySize; y++){
			for(int x = xStart; x < xStart + xSize; x++){
				Debug.Log ("TileWrite for FaceNumber: " + thisFace.faceNumber+ " Room Number: " +roomNumber);
				Debug.Log ("Passing X: " +x+ " and Y: " +y+ " to TileWrite");
				bool drawn = Architect.TileWrite(x,y,roomTileArray[x - xStart, y - yStart], thisFace);
				if (drawn) Debug.Log("!!!!!!Draw successful!!!!!!!!!!");
			}
		}
	}
}
