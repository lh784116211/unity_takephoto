using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;


public class Photo : MonoBehaviour
{
    public Transform target;//获取旋转目标
    public Camera mainCam; //待截图的目标摄像机
    RenderTexture rt;  //声明一个截图时候用的中间变量 
    Texture2D t2d;

    //Unity中是左手坐标系；
    private int r = 30;//球坐标径向距离为r；
    private int theta = 20; //OP的连线与正 Z 轴之间的夹角为天顶角θ;
    private int phi = 20; //OP的连线在 XY平面 上的投影线与正 X 轴之间的夹角为方位角φ。

    private float x = 0;
    private float y = 0;
    private float z = 0;

    Matrix4x4 matrix44 = Matrix4x4.identity;

    private string path = "";

    void Awake()
    {
        path = Application.dataPath + "/test.xml";



        //x = r * Mathf.Sin(theta * Mathf.PI / 180) * Mathf.Cos(phi * Mathf.PI / 180);
        //y = r * Mathf.Sin(theta * Mathf.PI / 180) * Mathf.Sin(phi * Mathf.PI / 180);
        //z = r * Mathf.Cos(theta * Mathf.PI / 180);

        //mainCam.transform.position = new Vector3(x, y, z);
        //mainCam.transform.LookAt(target.transform.position);
       
    }

    void Start()
    {
        t2d = new Texture2D(800, 600, TextureFormat.RGB24, false);
        rt = new RenderTexture(800, 600, 24);
        mainCam.targetTexture = rt;
        //TakePhoto(666);
        print(x + " " + y + " " + z);
    }

    float m_timer = 0;
    int xmlcnt = 0;
    int sequence=0;
    int a = 0;
    int s= 1;
    void Update()
    {
        m_timer += Time.time;//延时用；
        if (m_timer >= 0.5)
        {

            if (theta < 180)
            {
                if (phi < 360)
                {
                    x = r * Mathf.Sin(theta * Mathf.PI / 180) * Mathf.Cos(phi * Mathf.PI / 180);
                    y = r * Mathf.Sin(theta * Mathf.PI / 180) * Mathf.Sin(phi * Mathf.PI / 180);
                    z = r * Mathf.Cos(theta * Mathf.PI / 180);

                    mainCam.transform.position = new Vector3(x, y, z);
                    mainCam.transform.LookAt(target.transform.position);
                    matrix44 = mainCam.transform.localToWorldMatrix;

                    if (s%2==0) { 
                    TakePhoto(sequence);
                    Debug.Log("mainCam.position" + mainCam.transform.position+"R"+ mainCam.transform.eulerAngles );
                    //Debug.Log(Time.time );
                     phi = phi + 40;      
                                    }
                    s++;
                //Debug.Log("mainCam.transform.eulerAngles" + mainCam.transform.eulerAngles);

                   if (xmlcnt == 0)
                    {
                        Createcsxml(path, matrix44, sequence);
                        xmlcnt++;
                        sequence++;
                    }
                    else
                    {
                        AddcsXml(path, matrix44, sequence);
                        sequence++;
                    }
                   
                }
                else
                {
                    phi = 0;
                    theta = theta + 40;
                }

             }

        }
    }



    //void TakePhoto(Matrix4x4 matrix44) //拍照
    //{
    //    //pl.GetComponent<Renderer>().material.mainTexture = rt;

    //    //截图到t2d中
    //    RenderTexture.active = rt;
    //    t2d.ReadPixels(new Rect(0, 0, rt.width, rt.height), 0, 0);
    //    t2d.Apply();
    //    RenderTexture.active = null;

    //    //将图片保存起来
    //    byte[] byt = t2d.EncodeToJPG();

    //    File.WriteAllBytes(Application.dataPath + "//" + matrix44[0,3].ToString() + matrix44[1, 3].ToString() + ".jpg", byt);
    //    Debug.Log("当前截图序号为：" + matrix44[0, 3].ToString() + matrix44[1, 3].ToString());

    //}
    
    void TakePhoto(int sequence) //拍照
    {
        
        //截图到t2d中
        RenderTexture.active = rt;
        t2d.ReadPixels(new Rect(0, 0, rt.width, rt.height), 0, 0);
        t2d.Apply();
        RenderTexture.active = null;

        //将图片保存起来
        byte[] byt = t2d.EncodeToJPG();

        File.WriteAllBytes(Application.dataPath + "//" + ((sequence-1)/2).ToString() + ".jpg", byt);
 
    }



    private void OnApplicationQuit()
    {
        //TakePhoto(999);
    }

    void Createcsxml(string filePath, Matrix4x4 matrix)
    {

        XmlDocument doc = new XmlDocument();//实例化xmlDocument类
        XmlDeclaration dec = doc.CreateXmlDeclaration("1.0", null, null);

        //创建一个根节点（一级）
        XmlElement root = doc.CreateElement("opencv_storage");

        //创建节点（二级）
        XmlNode node = doc.CreateElement("Matrix4x4");
        XmlAttribute attr = doc.CreateAttribute("ID");
        attr.Value = matrix[0, 3].ToString() + "_" + matrix[1, 3].ToString() + "_" + matrix[2, 3].ToString();
        node.Attributes.Append(attr);
        node.Attributes.SetNamedItem(attr);

        //创建节点（三级）
        XmlElement element1 = doc.CreateElement("rows");
        element1.InnerText = "4";

        XmlElement element2 = doc.CreateElement("cols");
        element2.InnerText = "4";

        XmlElement element3 = doc.CreateElement("dt");
        element3.InnerText = "\"1F\"";

        XmlElement element4 = doc.CreateElement("data");
        element4.InnerText = matrix.ToString();

        node.AppendChild(element1);
        node.AppendChild(element2);
        node.AppendChild(element3);
        node.AppendChild(element4);

        root.AppendChild(node);

        doc.AppendChild(dec);
        doc.AppendChild(root);


        doc.Save(filePath);

    }

    void Createcsxml(string filePath, Matrix4x4 matrix,int sequence)
    {

        XmlDocument doc = new XmlDocument();//实例化xmlDocument类
        XmlDeclaration dec = doc.CreateXmlDeclaration("1.0", null, null);

        //创建一个根节点（一级）
        XmlElement root = doc.CreateElement("opencv_storage");

        //创建节点（二级）
        XmlNode node = doc.CreateElement("Matrix4x4");
        XmlAttribute attr = doc.CreateAttribute("ID");
        attr.Value = sequence.ToString();
        node.Attributes.Append(attr);
        node.Attributes.SetNamedItem(attr);

        //创建节点（三级）
        XmlElement element1 = doc.CreateElement("rows");
        element1.InnerText = "4";

        XmlElement element2 = doc.CreateElement("cols");
        element2.InnerText = "4";

        XmlElement element3 = doc.CreateElement("dt");
        element3.InnerText = "\"1F\"";

        XmlElement element4 = doc.CreateElement("data");
        element4.InnerText = matrix.ToString();

        node.AppendChild(element1);
        node.AppendChild(element2);
        node.AppendChild(element3);
        node.AppendChild(element4);

        root.AppendChild(node);

        doc.AppendChild(dec);
        doc.AppendChild(root);

        doc.Save(filePath);

    }


    void AddcsXml(string filePath, Matrix4x4 matrix)
    {
        if (File.Exists(path))
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(path);
            XmlNode root = doc.SelectSingleNode("opencv_storage");


            //创建节点（二级）
            XmlNode node = doc.CreateElement("Matrix4x4");
            XmlAttribute attr = doc.CreateAttribute("ID");
            attr.Value = matrix[0, 3].ToString() + "_" + matrix[1, 3].ToString() + "_" + matrix[2, 3].ToString();
            node.Attributes.Append(attr);
            node.Attributes.SetNamedItem(attr);

            //创建节点（三级）
            XmlElement element1 = doc.CreateElement("rows");
            element1.InnerText = "4";

            XmlElement element2 = doc.CreateElement("cols");
            element2.InnerText = "4";

            XmlElement element3 = doc.CreateElement("dt");
            element3.InnerText = "\"1F\"";

            XmlElement element4 = doc.CreateElement("data");
            element4.InnerText = matrix.ToString();

            node.AppendChild(element1);
            node.AppendChild(element2);
            node.AppendChild(element3);
            node.AppendChild(element4);
            root.AppendChild(node);
            doc.AppendChild(root);

            doc.Save(filePath);

        }
    }


    void AddcsXml(string filePath, Matrix4x4 matrix,int sequence)
    {
        if (File.Exists(path))
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(path);
            XmlNode root = doc.SelectSingleNode("opencv_storage");


            //创建节点（二级）
            XmlNode node = doc.CreateElement("Matrix4x4");
            XmlAttribute attr = doc.CreateAttribute("ID");
            attr.Value = sequence.ToString();
            node.Attributes.Append(attr);
            node.Attributes.SetNamedItem(attr);

            //创建节点（三级）
            XmlElement element1 = doc.CreateElement("rows");
            element1.InnerText = "4";

            XmlElement element2 = doc.CreateElement("cols");
            element2.InnerText = "4";

            XmlElement element3 = doc.CreateElement("dt");
            element3.InnerText = "\"1F\"";

            XmlElement element4 = doc.CreateElement("data");
            element4.InnerText = matrix.ToString();

            node.AppendChild(element1);
            node.AppendChild(element2);
            node.AppendChild(element3);
            node.AppendChild(element4);
            root.AppendChild(node);
            doc.AppendChild(root);

            doc.Save(filePath);

        }
    }







}
