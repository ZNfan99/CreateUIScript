using UnityEngine.Events;
using UnityEngine;
using UnityEngine.UI;
public class Img:MonoBehaviour
{
	public  Image Img_info;
	public string Img_Path="Canvas/Img";
	void Awake()
	{
	}
	void Start()
	{
		 Img_info=GetComponent< Image >();
	}
	void Update()
	{
	}
}
