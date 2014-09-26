using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SASDebug : SASSingleton<SASDebug>
{
	private Mesh _sphereMesh;
	private Material _diffuseMaterial;
	private int _defaultLayer;

	public override void Awake()
	{
		base.Awake();

		//using the Default layer
		_defaultLayer = Layers.Start().Add("Default");

		//default material is Diffuse
		_diffuseMaterial = new Material(Shader.Find("Diffuse"));

		//load default mesh and material
		GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
		sphere.SetActive(false);
		_sphereMesh = sphere.GetComponent<MeshFilter>().mesh;
		DestroyImmediate(sphere);
	}

	public void DrawSphere(Vector3 position, float radius, Color color)
	{
		DrawSphere(position, Vector3.one * radius * 2f, color);
	}
	public void DrawSphere(Vector3 position, Vector3 size, Color color)
	{
		MaterialPropertyBlock properties = new MaterialPropertyBlock();
		properties.AddColor("_Color", color);

		Matrix4x4 matrix = Matrix4x4.TRS(position, Quaternion.identity, size);

		Graphics.DrawMesh(_sphereMesh, matrix, _diffuseMaterial, _defaultLayer, null, 0, properties);
	}
}
