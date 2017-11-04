using UnityEngine;
using System.Collections;

public class Global : MonoBehaviour {

	public static string level_graphics;
	public static int [,] levelmatrix = new int[500, 500];
	public static int level_height;
	public static bool classic;
	public static int startcoord_x, startcoord_y, endcoord_x, endcoord_y;
	public static int inv_time=0;
	public static int ready_to_go;
}
