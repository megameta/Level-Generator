using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class FaceObj {

	public int cubeNumber;
	public int faceNumber;
	public int faceSize;
	public int[,] tileTypeArray;
	public Texture2D faceTexture;
	public GameObject faceGameObj;
	public MeshFilter faceMeshFilter;
	public MeshRenderer faceMeshRenderer;
	public MeshCollider faceMeshCollider;
	public int faceVertSize;
	public int faceVertNumber;
	public int faceTileNumber;
	public int faceTriNumber;
	public int faceZeroIndexSize;
	public Mesh faceMesh;
	public int cubeFaceLayer = 9;
	public int tileSize = 1;
	public CubeObj onCube;

	TerrainManager terrainManager;

	Vector3[] faceVerts;
	Vector3[] faceNormals;
	Vector2[] faceUV;
	int[] faceTriangles;
	
	public List<RoomObj> roomList = new List<RoomObj>();
	public List<TileObj> tileList = new List<TileObj>();
	
	public FaceObj(CubeObj thisCube, int faceNumber){
		onCube = thisCube;
		this.faceNumber = faceNumber;

		cubeNumber = thisCube.cubeNumber;
		faceSize = thisCube.cubeSize;
		faceZeroIndexSize = faceSize - 1;
		tileTypeArray = new int[faceSize, faceSize];
		faceGameObj = new GameObject();
		faceGameObj.name = "cubeFace" +faceNumber;
		faceGameObj.transform.parent = thisCube.cubeGameObj.transform;
		faceGameObj.layer = cubeFaceLayer;
		faceMeshFilter = faceGameObj.AddComponent<MeshFilter>();
		faceMeshRenderer = faceGameObj.AddComponent<MeshRenderer>();
		faceMeshCollider = faceGameObj.AddComponent<MeshCollider>();
		faceVertSize = faceSize + 1;
		faceVertNumber = faceVertSize * faceVertSize;
		faceTileNumber = faceSize * faceSize;
		faceTriNumber = faceTileNumber * 2;
		
		
		faceVerts = new Vector3[faceVertNumber];
		faceNormals = new Vector3[faceVertNumber];
		faceUV = new Vector2[faceVertNumber];
		faceTriangles = new int[faceTriNumber * 3];
		
		for(int y = 0; y < faceVertSize; y++){
			for(int x = 0; x < faceVertSize; x++){
				faceVerts[y * faceVertSize + x] = new Vector3(x * tileSize, y * tileSize, 0);
				faceNormals[y * faceVertSize + x] = Vector3.forward;
				faceUV[y * faceVertSize + x] = new Vector2((float)x / faceSize, (float)y / faceSize);
			}
		}
		
		for(int y = 0; y < faceSize; y++){
			for(int x = 0; x < faceSize; x++){
				int tileIndex = y * faceSize + x;
				int triangleOffset = tileIndex * 6;
			
				faceTriangles[triangleOffset + 0] = y * faceVertSize + x;
				faceTriangles[triangleOffset + 1] = y * faceVertSize + x + 1;
				faceTriangles[triangleOffset + 2] = y * faceVertSize + x + faceVertSize;
				
				faceTriangles[triangleOffset + 3] = y * faceVertSize + x + 1;
				faceTriangles[triangleOffset + 4] = y * faceVertSize + x + faceVertSize + 1;
				faceTriangles[triangleOffset + 5] = y * faceVertSize + x + faceVertSize;
				
			}
		}
		
		faceMesh = new Mesh();
		faceMesh.vertices = faceVerts;
		faceMesh.triangles = faceTriangles;
		faceMesh.normals = faceNormals;
		faceMesh.uv = faceUV;
		
		faceMeshFilter.mesh = faceMesh;
		faceMeshCollider.sharedMesh = faceMesh;

		faceMeshRenderer.material = Resources.Load<Material>("PreMaterial");

	}
}
