using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

[RequireComponent(typeof(Architect))]
public class TerrainManager : MonoBehaviour {
	
	public Texture2D tileAtlas;
	public int tileResolution;
	public int numCubeFaces = 6;
	public LayerMask cubeFaceLayerMask;
	public int[,,] cubePositionArray;
	public List<CubeObj> cubeMasterList = new List<CubeObj>();
	public List<FaceObj> faceMasterList = new List<FaceObj>();
	public List<RoomObj> roomMasterList = new List<RoomObj>();

	Vector3 levelSize;
	SceneBuilder sceneBuilder;
	
	void Awake(){
		sceneBuilder = GetComponent<SceneBuilder>();
		levelSize = sceneBuilder.levelSize;
		cubePositionArray = new int[(int)levelSize.x, (int)levelSize.y, (int)levelSize.z];
		tileAtlas = Resources.Load<Texture2D>("spriteAtlas");
		tileResolution = tileAtlas.height;
		levelSize = sceneBuilder.levelSize;
		cubePositionArray = new int[(int)levelSize.x, (int)levelSize.y, (int)levelSize.z];
	}

	public bool CheckForCube(Vector3 coords){
		if(cubePositionArray[(int)coords.x, (int)coords.y, (int)coords.z] == 1){
			return true;
		} else {
			return false;
		}
	}

	public float LatestDropInNeighborhood(Vector3 checkCoords){

		List<Vector3> neighborCoords = new List<Vector3>();

		neighborCoords.Add(new Vector3(checkCoords.x - 1, checkCoords.y - 1, checkCoords.z - 1));
		neighborCoords.Add(new Vector3(checkCoords.x - 0, checkCoords.y - 1, checkCoords.z - 1));
		neighborCoords.Add(new Vector3(checkCoords.x + 1, checkCoords.y - 1, checkCoords.z - 1));
		neighborCoords.Add(new Vector3(checkCoords.x - 1, checkCoords.y - 0, checkCoords.z - 1));
		neighborCoords.Add(new Vector3(checkCoords.x - 0, checkCoords.y - 0, checkCoords.z - 1));
		neighborCoords.Add(new Vector3(checkCoords.x + 1, checkCoords.y - 0, checkCoords.z - 1));
		neighborCoords.Add(new Vector3(checkCoords.x - 1, checkCoords.y + 1, checkCoords.z - 1));
		neighborCoords.Add(new Vector3(checkCoords.x - 0, checkCoords.y + 1, checkCoords.z - 1));
		neighborCoords.Add(new Vector3(checkCoords.x + 1, checkCoords.y + 1, checkCoords.z - 1));
		neighborCoords.Add(new Vector3(checkCoords.x - 1, checkCoords.y - 1, checkCoords.z - 0));
		neighborCoords.Add(new Vector3(checkCoords.x - 0, checkCoords.y - 1, checkCoords.z - 0));
		neighborCoords.Add(new Vector3(checkCoords.x + 1, checkCoords.y - 1, checkCoords.z - 0));
		neighborCoords.Add(new Vector3(checkCoords.x - 1, checkCoords.y - 0, checkCoords.z - 0));
		neighborCoords.Add(new Vector3(checkCoords.x - 0, checkCoords.y - 0, checkCoords.z - 0));
		neighborCoords.Add(new Vector3(checkCoords.x + 1, checkCoords.y - 0, checkCoords.z - 0));
		neighborCoords.Add(new Vector3(checkCoords.x - 1, checkCoords.y + 1, checkCoords.z - 0));
		neighborCoords.Add(new Vector3(checkCoords.x - 0, checkCoords.y + 1, checkCoords.z - 0));
		neighborCoords.Add(new Vector3(checkCoords.x + 1, checkCoords.y + 1, checkCoords.z - 0));
		neighborCoords.Add(new Vector3(checkCoords.x - 1, checkCoords.y - 1, checkCoords.z + 1));
		neighborCoords.Add(new Vector3(checkCoords.x - 0, checkCoords.y - 1, checkCoords.z + 1));
		neighborCoords.Add(new Vector3(checkCoords.x + 1, checkCoords.y - 1, checkCoords.z + 1));
		neighborCoords.Add(new Vector3(checkCoords.x - 1, checkCoords.y - 0, checkCoords.z + 1));
		neighborCoords.Add(new Vector3(checkCoords.x - 0, checkCoords.y - 0, checkCoords.z + 1));
		neighborCoords.Add(new Vector3(checkCoords.x + 1, checkCoords.y - 0, checkCoords.z + 1));
		neighborCoords.Add(new Vector3(checkCoords.x - 1, checkCoords.y + 1, checkCoords.z + 1));
		neighborCoords.Add(new Vector3(checkCoords.x - 0, checkCoords.y + 1, checkCoords.z + 1));
		neighborCoords.Add(new Vector3(checkCoords.x + 1, checkCoords.y + 1, checkCoords.z + 1));

		float latestDropTime = 0f;

		foreach(Vector3 cubeCoords in neighborCoords){
			CubeObj cube = cubeMasterList.FirstOrDefault(i => i.cubeArrayPosition == cubeCoords);
			if(cube != null){
				float dropTime = cube.dropTime;
				if (dropTime > latestDropTime){
					latestDropTime = dropTime;
				}
			}
		}

		return latestDropTime;

	}
}
