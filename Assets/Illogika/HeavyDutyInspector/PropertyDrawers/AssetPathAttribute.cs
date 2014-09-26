//----------------------------------------------
//            Heavy-Duty Inspector
//      Copyright © 2013 - 2014  Illogika
//----------------------------------------------

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace HeavyDutyInspector
{

	public enum PathOptions {
		RelativeToAssets,
		RelativeToResources,
		FilenameOnly
	}

	public class AssetPathAttribute : PropertyAttribute {

		public UnityEngine.Object obj
		{
			get; set;
		}

		public System.Type type
		{
			get;
			private set;
		}

		public PathOptions pathOptions
		{
			get;
			private set;
		}

		/// <summary>
		/// Displays a strings as a reference to get the asset's path without risking typing errors.
		/// </summary>
		/// <param name="type">The asset's type.</param>
		/// <param name="pathOptions">The way your path should be formatted. Relative to the Assets folder, relative to a Resources folder and with no extension, or just the filename.</param>
		public AssetPathAttribute(System.Type type, PathOptions pathOptions)
		{
			this.pathOptions = pathOptions;
			this.type = type;
		}

		/// <summary>
		/// Displays a strings as a reference to get the asset's path without risking typing errors. Will accept any asset.
		/// </summary>
		/// <param name="pathOptions">The way your path should be formatted. Relative to the Assets folder, relative to a Resources folder and with no extension, or just the filename.</param>
		public AssetPathAttribute(PathOptions pathOptions)
		{
			this.pathOptions = pathOptions;
			this.type = typeof(UnityEngine.Object);
		}
	}
}