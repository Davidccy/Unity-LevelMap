using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

public class EditorRemoveWhite : MonoBehaviour {

    [MenuItem("PPP/RemoveWhite")]
    public static void RemoveWhite() {
        byte[] bytes = File.ReadAllBytes("Assets/Sprites/Wateri.png");
        Texture2D baseTex = new Texture2D(256, 256, TextureFormat.ARGB32, false);
        baseTex.LoadImage(bytes);

        // Output
        Texture2D newTex = new Texture2D(baseTex.width, baseTex.height, baseTex.format, false);
        for (int w = 0; w < baseTex.width; w++) {
            for (int h = 0; h < baseTex.height; h++) {
                Color c = baseTex.GetPixel(w, h);
                if (c == Color.white) {
                    c.a = 0;
                }

                newTex.SetPixel(w, h, c);
            }
        }

        //
        byte[] newBytes = newTex.EncodeToPNG();
        File.WriteAllBytes("Assets/Sprites/NewWateri.png", newBytes);
        AssetDatabase.Refresh();
    }
}
