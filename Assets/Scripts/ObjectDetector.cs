using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OpenCvSharp;
using System;
using System.Runtime.InteropServices;
using Uk.Org.Adcock.Parallel;
using System.IO;
using UnityEngine.UI;
//using System.Windows.Forms;

public class ObjectDetector : MonoBehaviour {

    [DllImport("user32.dll")]
    private static extern void OpenFileDialog();

    public MeshRenderer quadRender;
    private Texture2D processedTexture;
    public InputField path_txt;
    private float cube_pos_x;
    private float cube_pos_y;
    private GameObject rectangle;		private float rec_z;
	private GameObject triangularity;	private float tri_z;
    private GameObject sphere;			private float sph_z;
    private GameObject pipe;			private float pipe_z;
	private GameObject cylinder;		private float cyl_z;
	private GameObject tank;			private float tnk_z;
	private GameObject polygon;			private float pol_z;
    private float iWidth = 640;
	private float iHeight = 480;
	private string filePath = string.Empty;
	public Button path_button;
	private List<GameObject> Graphic_list = new List<GameObject>();

    public enum GraphicType  {
        GT_Rectangle,
        GT_Triangularity,
        GT_Sphere,
        GT_Pipe,
        GT_Cylinder,
        GT_TNK,
		GT_Polygon
    };

	public struct Polygon_T
	{
		public Point center_pos;
		public GraphicType gt;
	}

	public List<Polygon_T> polygon_array = new List<Polygon_T>();

    void Start ()
    {
        //initiate Graphics
        rectangle = GameObject.Find("rectangle");		rec_z = rectangle.transform.localPosition.z;
		triangularity = GameObject.Find("triangle");	tri_z = triangularity.transform.localPosition.z;
		sphere = GameObject.Find("sphere");				sph_z = sphere.transform.localPosition.z;
		pipe = GameObject.Find("rectangle");			pipe_z = pipe.transform.localPosition.z;
		cylinder = GameObject.Find("rectangle");		cyl_z = cylinder.transform.localPosition.z;
		tank = GameObject.Find("rectangle");			tnk_z = tank.transform.localPosition.z;
		polygon = GameObject.Find ("polygon");			pol_z = polygon.transform.localPosition.z;

		rectangle.SetActive(false);
		sphere.SetActive(false);
		pipe.SetActive(false);
		triangularity.SetActive(false);
		cylinder.SetActive(false);
		tank.SetActive(false);
		polygon.SetActive (false);

		Button btn = path_button.GetComponent<Button>();
		btn.onClick.AddListener (TaskOnClick);

    }

	void Update()
	{
        if (Input.GetKeyDown(KeyCode.F2))
        {
            TaskOnClick();
        }
	}
	public void TaskOnClick()
	{

		foreach (GameObject go in Graphic_list) {			
			Destroy (go.gameObject);
		}

		polygon_array.Clear ();

		processedTexture = LoadPNG();

        if (processedTexture != null)
		    openCV_method ();

	}

    public Texture2D LoadPNG()
    {
        Texture2D tex = null;
        byte[] fileData;

        filePath = path_txt.text;
        if (File.Exists (filePath)) 
		{
			fileData = File.ReadAllBytes (filePath);
			tex = new Texture2D (2, 2);
			tex.LoadImage (fileData); //..this will auto-resize the texture dimensions.
		} 

        return tex;
    }

	void DrawGraphic(Polygon_T pointinfo)
    {
		cube_pos_x = (float)pointinfo.center_pos.X - iWidth / 2;
		cube_pos_y = (float)pointinfo.center_pos.Y - iHeight / 2;
        cube_pos_x = cube_pos_x / 100.0f;
        cube_pos_y = cube_pos_y / 100.0f * (-1.0f);

		switch (pointinfo.gt)
        {
            case GraphicType.GT_Rectangle:
			rectangle.transform.localPosition = new Vector3(cube_pos_x, cube_pos_y, rec_z);
				GameObject obj_rectangle = Instantiate(rectangle);
				obj_rectangle.transform.position = new Vector3(cube_pos_x, cube_pos_y, rec_z);
				Graphic_list.Add (obj_rectangle);

                break;
            case GraphicType.GT_Triangularity:
				triangularity.transform.localPosition = new Vector3(cube_pos_x, cube_pos_y, tri_z);
				GameObject obj_triangule = Instantiate(triangularity);
				obj_triangule.transform.position = new Vector3(cube_pos_x, cube_pos_y, tri_z);
				Graphic_list.Add (obj_triangule);

                break;
			case GraphicType.GT_Sphere:
				sphere.transform.localPosition = new Vector3(cube_pos_x, cube_pos_y, sph_z);
				GameObject obj_sphere = Instantiate(sphere);
				obj_sphere.transform.position = new Vector3(cube_pos_x, cube_pos_y, sph_z);
				Graphic_list.Add (obj_sphere);

                break;
            case GraphicType.GT_Pipe:
				pipe.transform.localPosition = new Vector3(cube_pos_x, cube_pos_y, pipe_z);
				GameObject obj_pipe = Instantiate(pipe);
				obj_pipe.transform.position = new Vector3(cube_pos_x, cube_pos_y, pipe_z);
				Graphic_list.Add (obj_pipe);

                break;
            case GraphicType.GT_Cylinder:
				cylinder.transform.localPosition = new Vector3(cube_pos_x, cube_pos_y, cyl_z);
				GameObject obj_cylinder = Instantiate(cylinder);
				obj_cylinder.transform.position = new Vector3(cube_pos_x, cube_pos_y, cyl_z);
				Graphic_list.Add (obj_cylinder);

                break;
            case GraphicType.GT_TNK:
				tank.transform.localPosition = new Vector3(cube_pos_x, cube_pos_y, tnk_z);
				GameObject obj_tank = Instantiate(tank);
				obj_tank.transform.position = new Vector3(cube_pos_x, cube_pos_y, tnk_z);
				Graphic_list.Add (obj_tank);

                break;
			case GraphicType.GT_Polygon:
				polygon.transform.localPosition = new Vector3(cube_pos_x, cube_pos_y, pol_z);
				GameObject obj_polygon = Instantiate(polygon);
				obj_polygon.transform.position = new Vector3(cube_pos_x, cube_pos_y, pol_z);
				Graphic_list.Add (obj_polygon);

				break;

        }
    }

    ///////////////////////////////////////////////////////////////////////////////////////////////
	void openCV_method()
	{
		rectangle.SetActive(true);
		triangularity.SetActive (true);
		sphere.SetActive (true);
		polygon.SetActive (true);

		quadRender.material.mainTexture = processedTexture;

		var src = new Mat(filePath, ImreadModes.AnyDepth | ImreadModes.AnyColor).Resize(new Size(640, 480));
		var srcCopy = new Mat();
		src.CopyTo(srcCopy);

		var image_gray = new Mat();
		var image_canny = new Mat();
		var image_circle = new Mat();
		Cv2.CvtColor(srcCopy, image_gray, ColorConversionCodes.BGRA2GRAY);
		image_gray.CopyTo(image_circle);
		Cv2.Canny(image_gray, image_gray, 150, 250, 3);

		Cv2.Blur(image_gray, image_canny, new Size(3, 3));

		var kenel = new Mat();
		kenel = Cv2.GetStructuringElement(MorphShapes.Ellipse, new Size(11, 11));
		var morph = new Mat();
		Cv2.MorphologyEx(image_gray, morph, MorphTypes.Close, kenel);

		Point[][] contours = new Point[100][];
		HierarchyIndex[] hierarchy = new HierarchyIndex[100];

		Cv2.FindContours(morph, out contours, out hierarchy, RetrievalModes.CComp,
			ContourApproximationModes.ApproxSimple, new Point(0, 0));

		int idx;
		Mat contours_img = new Mat();

		Cv2.CvtColor(image_gray, contours_img, ColorConversionCodes.GRAY2BGR);
		for (idx = 0; idx < contours.Length; idx++)
		{
			Cv2.DrawContours(contours_img, contours, idx, Scalar.Red, 2);
		}


        Mat poly_img = new Mat();
		Mat ractangle_img = new Mat();

		Cv2.CvtColor(image_gray, poly_img, ColorConversionCodes.GRAY2BGR);
		Cv2.CvtColor(image_gray, ractangle_img, ColorConversionCodes.GRAY2BGR);

		find_circle(image_circle, 50.0f);
		find_polygon(3, 50.0f, contours, image_circle);
		find_polygon(4, 50.0f, contours, image_circle);
        find_polygon(10000, 50.0f, contours, image_circle);

        foreach (Polygon_T s in polygon_array) {
			DrawGraphic(s);
		}


		rectangle.SetActive(false);
		triangularity.SetActive (false);
		sphere.SetActive (false);
		polygon.SetActive (false);
		filePath = string.Empty;
	}

	public void find_circle(Mat img_mat, double threadhold)
	{
		Cv2.Blur(img_mat, img_mat, new Size(5, 5));
		CircleSegment[] circle_seg = new CircleSegment[100];
		circle_seg = Cv2.HoughCircles(img_mat, HoughMethods.Gradient, 1, 100, 200, 40, 20);
		for (int i = 0; i < circle_seg.Length; i++)
		{
			bool bfound = false;
			foreach (Polygon_T s in polygon_array)
			{
				if (circle_seg[i].Center.X > 0 && circle_seg[i].Center.Y > 0)
				{
					double dist = Math.Sqrt((circle_seg[i].Center.X - s.center_pos.X) * 
						(circle_seg[i].Center.X - s.center_pos.X) + 
						(circle_seg[i].Center.Y - s.center_pos.Y) * 
						(circle_seg[i].Center.Y - s.center_pos.Y));
					if (dist < threadhold)
					{
						bfound = true;
					}
				}
			}

			if (bfound == false)
			{
				Polygon_T p = new Polygon_T();
				p.center_pos = circle_seg[i].Center;
				p.gt = GraphicType.GT_Sphere;
				polygon_array.Add(p);

				Cv2.Circle(img_mat, circle_seg[i].Center, 5, Scalar.Red, 2);
				Cv2.Circle(img_mat, (int)circle_seg[i].Center.X, (int)circle_seg[i].Center.Y, (int)circle_seg[i].Radius, Scalar.Red, 2);
			}
		}
	}

	public void find_polygon(int polygon_type, double threadhold, Point[][] contours, Mat result_mat)
	{   
		int idx, i;
		Point[] poly = new Point[100];
		Point center_pos = new Point();

		center_pos.X = 0;
		center_pos.Y = 0;

		for (idx = 0; idx < contours.Length; idx++)
		{
			poly = Cv2.ApproxPolyDP(contours[idx], 10, true);
            if (polygon_type == 10000)
            {
                int left = 10000, right = -10000, top = 10000, bottom = -10000;
                for (i = 0; i < poly.Length; i++)
                {
                    if (left > poly[i].X) left = poly[i].X;
                    if (right < poly[i].X) right = poly[i].X;
                    if (top > poly[i].Y) top = poly[i].Y;
                    if (bottom < poly[i].Y) bottom = poly[i].Y;

                    center_pos.X += poly[i].X;
                    center_pos.Y += poly[i].Y;
                    Cv2.Line(result_mat, poly[i], poly[(i + 1) % poly.Length], Scalar.Blue, 2);
                }

                if (right - left > 30)
                {
                    center_pos.X = (left + right) / 2;
                    center_pos.Y = (top + bottom) / 2;

                    bool bfound = false;
                    foreach (Polygon_T s in polygon_array)
                    {
                        if (center_pos.X > 0 && center_pos.Y > 0)
                        {
                            double dist = Math.Sqrt((center_pos.X - s.center_pos.X) *
                                (center_pos.X - s.center_pos.X) +
                                (center_pos.Y - s.center_pos.Y) *
                                (center_pos.Y - s.center_pos.Y));
                            if (dist < threadhold)
                            {
                                bfound = true;
                            }
                        }
                    }

                    if (bfound == false)
                    {
                        Polygon_T p = new Polygon_T();
                        p.center_pos = center_pos;
                        p.gt = GraphicType.GT_Polygon;

                        polygon_array.Add(p);

                        Cv2.Circle(result_mat, center_pos, 5, Scalar.Red, 2);
                    }
                }
                }
            else
            {
                if (poly.Length == polygon_type)
                {
                    int left = 10000, right = -10000, top = 10000, bottom = -10000;
                    for (i = 0; i < poly.Length; i++)
                    {
                        if (left > poly[i].X) left = poly[i].X;
                        if (right < poly[i].X) right = poly[i].X;
                        if (top > poly[i].Y) top = poly[i].Y;
                        if (bottom < poly[i].Y) bottom = poly[i].Y;

                        center_pos.X += poly[i].X;
                        center_pos.Y += poly[i].Y;
                        Cv2.Line(result_mat, poly[i], poly[(i + 1) % poly.Length], Scalar.Blue, 2);
                    }

                    center_pos.X = (left + right) / 2;
                    center_pos.Y = (top + bottom) / 2;

                    bool bfound = false;
                    foreach (Polygon_T s in polygon_array)
                    {
                        if (center_pos.X > 0 && center_pos.Y > 0)
                        {
                            double dist = Math.Sqrt((center_pos.X - s.center_pos.X) *
                                (center_pos.X - s.center_pos.X) +
                                (center_pos.Y - s.center_pos.Y) *
                                (center_pos.Y - s.center_pos.Y));
                            if (dist < threadhold)
                            {
                                bfound = true;
                            }
                        }
                    }

                    if (bfound == false)
                    {
                        Polygon_T p = new Polygon_T();
                        p.center_pos = center_pos;
                        if (polygon_type == 3) p.gt = GraphicType.GT_Triangularity;
                        if (polygon_type == 4) p.gt = GraphicType.GT_Rectangle;

                        polygon_array.Add(p);

                        Cv2.Circle(result_mat, center_pos, 5, Scalar.Red, 2);
                    }
                }
            }
		}
	}
}
