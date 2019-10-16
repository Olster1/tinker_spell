using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Assertions;

public class MyAssetBundle {
	public Object[] objs;
	public AssetBundle bundle;

}

public class MyAssetBundleManager: MonoBehaviour
{
	
	public IDictionary<string, MyAssetBundle> assetBundles;

	public void Awake () {
		 assetBundles = new Dictionary<string, MyAssetBundle>();
	}

	public MyAssetBundle LoadBundle(string assetBundleName) {
		Assert.IsTrue(assetBundles != null);
		MyAssetBundle result;
		if(!assetBundles.ContainsKey(assetBundleName)) {
			AssetBundle myLoadedAssetBundle = AssetBundle.LoadFromFile(Path.Combine(Application.streamingAssetsPath, assetBundleName)); 
			if (myLoadedAssetBundle == null) { 
				Debug.Log("Failed to load AssetBundle!"); 
			}

			MyAssetBundle toAdd = new MyAssetBundle();
			toAdd.objs = myLoadedAssetBundle.LoadAllAssets();
			toAdd.bundle = myLoadedAssetBundle;

			assetBundles.Add(assetBundleName, toAdd);
			result = toAdd;

		} else {
			result = assetBundles[assetBundleName];
		}
		return result;
	}

	public Object[] GetAssetsOfType(MyAssetBundle a, System.Type type) {
		Object[] result = new Object[a.objs.Length];
		int count = 0;
		//@speed
		for(int i = 0; i < a.objs.Length; ++i) {
			Object o = a.objs[i];
			// if(type.GetType().Equals(typeof(UnityEngine.RuntimeAnimatorController).GetType())) {
			// 	Debug.Log("HRY GO");
			// }

			if(o.GetType().Equals(type) || o.GetType().BaseType.Equals(type)) {
				Debug.Log("HRY" + o.GetType());
				Debug.Log("HRY" + type);
				result[count++] = o;
			}
		}
		return result;
	}


	public void UnloadAssetBundle(string assetBundleName) {
		assetBundles[assetBundleName].bundle.Unload(false); 
		assetBundles.Remove(assetBundleName);
	}
}
