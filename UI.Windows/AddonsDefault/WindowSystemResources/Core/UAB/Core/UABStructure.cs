﻿using UnityEngine;
using System.Collections;

namespace ME.UAB {

	public class BundleIgnoreAttribute : System.Attribute {
	};

	public enum FieldType : byte {

		ValueType,
		ReferenceType,
		NestedType,
		ArrayType,
		BinaryType,

	};

	public class UABReference {

		public string instanceId;
		public bool isLocal;
		public bool isGameObject;

	}

	public class UABBinary {

		public string instanceId;

	}

	public class UABField {

		public string name;
		public FieldType fieldType;
		public int serializatorId;
		public UABField[] fields;
		public string data;

	}

	public class UABComponent {

		public int instanceId;
		public string type;
		public UABField[] fields;

	}

	public class UABBinaryData {

		public string instanceId;
		public string data;

	}

	public class UABBinaryHeader {

		public string instanceId;
		public UABField field;
		public string binDataInstanceId;

	}

	public class UABGameObject {

		public UABComponent[] components;
		public UABGameObject[] childs;

		public string name;
		public string tag;
		public int layer;
		public bool active;

	}

	public class UABPackage {

		public UABGameObject[] objects;
		public UABBinaryHeader[] binaryHeaders;
		public UABBinaryData[] binaryData;

	}

}