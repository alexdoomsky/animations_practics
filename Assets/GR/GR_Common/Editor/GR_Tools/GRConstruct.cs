/********************************************************************
 * GRConstruct v1r01 by Vit3D 2015
 * See GRConstruct_ReadMe.txt for instructions.
 * ******************************************************************/
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

public class GRConstruct : EditorWindow {
// Input Parameters
	public GameObject avatar = null;
	public GameObject newPart = null;
	public bool autoprefab = false;
	public string message = "Define parameters :";

	// Init GUI
    [MenuItem("GR Tools/GR Construct")]
	public static void ShowWindow()
	{
		//Show existing window instance. If one doesn't exist, make one.
		EditorWindow editorWindow = EditorWindow.GetWindow(typeof(GRConstruct));
		editorWindow.autoRepaintOnSceneChange = true;
		editorWindow.Show();
	}	

	// GUI Processing
    void OnGUI() {
		var all_pars = true;
		// GUI layout
		EditorGUILayout.LabelField ("GRConstruct v1r01 © 2015 Vit3D", EditorStyles.centeredGreyMiniLabel);
		EditorGUI.LabelField (new Rect(3, 25, position.width - 6, 20), message, EditorStyles.boldLabel);
		avatar = (GameObject) EditorGUI.ObjectField(new Rect(3, 50, position.width - 6, 20), "Base Avatar", avatar, typeof(GameObject), true);
		newPart = (GameObject) EditorGUI.ObjectField(new Rect(3, 75, position.width - 6, 20), "Part to Add", newPart, typeof(GameObject), true);
		autoprefab = EditorGUI.Toggle(new Rect(3,100,position.width - 6,20),"Auto Prefabs ", autoprefab);
		// Process Button
		if(GUI.Button(new Rect(3, 125, position.width - 6, 30),"Add Part to Avatar"))
		{
			if (avatar == null) all_pars = false;
			if (newPart == null) all_pars = false;
			if (all_pars == false) {
				message = "Define all(!) parameters :";
			}
			else {
				NewPartAdd(avatar, newPart);
				if (autoprefab) 
					CreatePrefab(avatar, "Assets/" + avatar.name + "_" + newPart.name + ".prefab");
				message = "Done. You can define new Part to Add :";
				newPart = null;
				all_pars = true;
			}
		}
	}

// ================== Functions ==================
// Creating Prefab from combined avatar and adding it to Assets.
	private void CreatePrefab(GameObject avatarnew, string filename)
	{
		var prefab = PrefabUtility.CreateEmptyPrefab(filename);
		PrefabUtility.ReplacePrefab(avatar, prefab);
		AssetDatabase.Refresh();	
	}

// Adding NewPart to Avatar (main function).
	private GameObject NewPartAdd (GameObject baseAvatar, GameObject partSource)
	{
		var boneCatalog = new TransformCatalog (baseAvatar.transform);
		var skinnedMeshRenderers = partSource.GetComponentsInChildren<SkinnedMeshRenderer> ();
		var partNew = AddChild (partSource, baseAvatar.transform);
		foreach (var sourceRenderer in skinnedMeshRenderers) {
			var targetRenderer = AddSMRenderer (baseAvatar, partNew, sourceRenderer);
			targetRenderer.bones = TranslateTransforms (sourceRenderer.bones, boneCatalog);
		}
		return partNew;
	}
// Add newPart as child to Avatar.
	private GameObject AddChild (GameObject source, Transform parent)
	{
		var target = new GameObject (source.name);
		target.transform.parent = parent;
		target.transform.localPosition = source.transform.localPosition;
		target.transform.localRotation = source.transform.localRotation;
		target.transform.localScale = source.transform.localScale;
		return target;
	}
// Copy SkinnedMeshRenderer for newPart.
	private SkinnedMeshRenderer AddSMRenderer (GameObject baseAvatar, GameObject parent, SkinnedMeshRenderer source)
	{
		Transform root_bone = null;
		var target = parent.AddComponent<SkinnedMeshRenderer> ();
		target.sharedMesh = source.sharedMesh;
		target.sharedMaterials = source.sharedMaterials;
		target.localBounds = source.localBounds;
		target.updateWhenOffscreen = true;
		root_bone = source.rootBone;
//		Debug.Log ("Root Bone : " + root_bone.gameObject.name);
		root_bone = FindBoneByName(baseAvatar, root_bone.gameObject.name);
		target.rootBone = root_bone;
		return target;
	}
// Find New Root Bone
	private Transform FindBoneByName (GameObject gobj, string bone_name)
	{
		Transform bone_found = null;
		Transform[] ts = gobj.transform.GetComponentsInChildren<Transform>(true);
		foreach (Transform t in ts) {
			if (t.gameObject.name == bone_name) {
				bone_found = t;
				break;
			}
		}
		return bone_found;
	}

	private Transform[] TranslateTransforms (Transform[] sources, TransformCatalog transformCatalog)
	{
		var targets = new Transform[sources.Length];
		for (var index = 0; index < sources.Length; index++)
			targets [index] = DictionaryExtensions.Find (transformCatalog, sources [index].name);
		return targets;
	}

// TransformCatalog
	private class TransformCatalog : Dictionary<string, Transform>
	{
		public TransformCatalog (Transform transform)
		{
			Catalog (transform);
		}
		private void Catalog (Transform transform)
		{
			Add (transform.name, transform);
			foreach (Transform child in transform)
				Catalog (child);
		}
	}

// DictionaryExtensions
	private class DictionaryExtensions
	{
		public static TValue Find<TKey, TValue> (Dictionary<TKey, TValue> source, TKey key)
		{
			TValue value;
			source.TryGetValue (key, out value);
			return value;
		}
	}
}