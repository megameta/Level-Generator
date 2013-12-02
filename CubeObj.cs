using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class CubeObj {

	public int cubeNumber;
	public int cubeSize;
	public int numFaces;
	public bool interior;
	public GameObject cubeGameObj;
	public Vector3 cubeArrayPosition;
	public bool heldByAgent;
	public float dropTime;
	public float positionSum;
	public int networkSize;

	public List<FaceObj> faceList = new List<FaceObj>();
	public List<RoomObj> roomList = new List<RoomObj>();

	public CubeObj(int cubeNumber, int cubeSize, bool interior, Vector3 cubeArrayPosition){
		this.cubeNumber = cubeNumber;
		this.cubeSize = cubeSize;
		this.cubeArrayPosition = cubeArrayPosition;
		heldByAgent = false;
		dropTime = Time.time;
		
		cubeGameObj = new GameObject();
		cubeGameObj.name = "cube" +cubeNumber;
		
		if(interior){
				cubeGameObj.transform.position = new Vector3(cubeSize * .5f, cubeSize * .5f, -cubeSize * .5f);
			} else {
				cubeGameObj.transform.position = new Vector3(cubeSize * .5f, cubeSize * .5f, cubeSize * .5f);
			}	
		
		for(int face = 0; face < 6; face++){
			FaceObj newFace = new FaceObj(this, face);
			faceList.Add (newFace);
		}
		
		// Postion the face meshes as an inverted cross to prepare for either internal or external wrapping to form a cube
		
		foreach(FaceObj face in faceList){
		
			switch(face.faceNumber){
				case 0:
					break;
				case 1:
					face.faceGameObj.transform.position = face.faceGameObj.transform.position + new Vector3(face.faceSize, 0, 0);
					break;
				case 2:
					face.faceGameObj.transform.position = face.faceGameObj.transform.position + new Vector3(0, -face.faceSize, 0);	
					break;
				case 3:
					face.faceGameObj.transform.position = face.faceGameObj.transform.position + new Vector3(-face.faceSize, 0, 0);
					break;
				case 4:
					face.faceGameObj.transform.position = face.faceGameObj.transform.position + new Vector3(0, face.faceSize, 0);
					break;
				case 5:
					face.faceGameObj.transform.position = face.faceGameObj.transform.position + new Vector3(0, face.faceSize * 2, 0);
					break;
			}
		}
		
		//Wrap the face meshes to form a cube	
		
		if(interior){
			faceList[1].faceGameObj.transform.RotateAround(faceList[1].faceGameObj.transform.position, faceList[1].faceGameObj.transform.up, 90f);
			faceList[2].faceGameObj.transform.RotateAround(faceList[0].faceGameObj.transform.position, faceList[0].faceGameObj.transform.right, 90f);
			faceList[3].faceGameObj.transform.RotateAround(faceList[0].faceGameObj.transform.position, faceList[0].faceGameObj.transform.up, -90f);
			faceList[4].faceGameObj.transform.RotateAround(faceList[4].faceGameObj.transform.position, faceList[4].faceGameObj.transform.right, -90f);
			faceList[5].faceGameObj.transform.RotateAround(faceList[4].faceGameObj.transform.position, faceList[4].faceGameObj.transform.right, -90f);
			faceList[5].faceGameObj.transform.RotateAround(faceList[5].faceGameObj.transform.position, faceList[5].faceGameObj.transform.right, -90f);
		} else {
			faceList[1].faceGameObj.transform.RotateAround(faceList[1].faceGameObj.transform.position, faceList[1].faceGameObj.transform.up, -90f);
			faceList[2].faceGameObj.transform.RotateAround(faceList[0].faceGameObj.transform.position, faceList[0].faceGameObj.transform.right, -90f);
			faceList[3].faceGameObj.transform.RotateAround(faceList[0].faceGameObj.transform.position, faceList[0].faceGameObj.transform.up, 90f);
			faceList[4].faceGameObj.transform.RotateAround(faceList[4].faceGameObj.transform.position, faceList[4].faceGameObj.transform.right, 90f);
			faceList[5].faceGameObj.transform.RotateAround(faceList[4].faceGameObj.transform.position, faceList[4].faceGameObj.transform.right, 90f);
			faceList[5].faceGameObj.transform.RotateAround(faceList[5].faceGameObj.transform.position, faceList[5].faceGameObj.transform.right, 90f);
		}
	}

	public void PositionSum(){
		positionSum = cubeGameObj.transform.position.x + cubeGameObj.transform.position.y + cubeGameObj.transform.position.z;
	}
}
