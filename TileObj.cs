using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class TileObj : MonoBehaviour {
	
	FaceObj faceObj;
	RoomObj roomObj;
	int tileType;
	
	public static Dictionary<string, int> TileDict = new Dictionary<string, int>(){
		{"unknown", 0},
		{"floor", 1},
		{"wall", 2},
		{"water", 3},
		{"upstairs", 4},
		{"downstairs", 5}
	};
	
	public TileObj(int tileType, FaceObj face, RoomObj room){
		this.tileType = tileType;
		faceObj = face;
		roomObj = room;	
	}	
}
