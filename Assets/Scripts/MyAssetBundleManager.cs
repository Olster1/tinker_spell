using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class AssetBundleManager
{
	public IDictionary<string, AssetBundle> assetBundles;

	public AssetBundleManager() {
		 assetBundles = new Dictionary<string, AssetBundle>();
	}
	public void LoadBundle(string assetBundleName) {
		AssetBundle myLoadedAssetBundle = AssetBundle.LoadFromFile(Path.Combine(Application.streamingAssetsPath, assetBundleName)); 
		if (myLoadedAssetBundle == null) { 
			Debug.Log("Failed to load AssetBundle!"); 
			return; 
		}

		assetBundles.Add(assetBundleName, myLoadedAssetBundle);

		myLoadedAssetBundle.LoadAllAssets();//Just load everything in the bundle


	}


	public void UnloadAssetBundle(string assetBundleName) {

		assetBundles[assetBundleName].Unload(false); 
		assetBundles.Remove(assetBundleName);
	}
}
