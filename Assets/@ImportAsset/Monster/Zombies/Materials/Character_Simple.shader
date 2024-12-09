// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Shader Forge/Character"
{
	Properties
	{
		_Cutoff( "Mask Clip Value", Float ) = 0.5
		_T_Pandemic01_BC("T_Pandemic01_BC", 2D) = "white" {}
		_T_Pandemic01_ORM("T_Pandemic01_ORM", 2D) = "white" {}
		_T_Pandemic01_N("T_Pandemic01_N", 2D) = "bump" {}
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "TransparentCutout"  "Queue" = "Geometry+0" }
		Cull Back
		CGPROGRAM
		#pragma target 3.0
		#pragma surface surf Standard keepalpha addshadow fullforwardshadows 
		struct Input
		{
			float2 uv_texcoord;
		};

		uniform sampler2D _T_Pandemic01_N;
		uniform float4 _T_Pandemic01_N_ST;
		uniform sampler2D _T_Pandemic01_BC;
		uniform float4 _T_Pandemic01_BC_ST;
		uniform sampler2D _T_Pandemic01_ORM;
		uniform float4 _T_Pandemic01_ORM_ST;
		uniform float _Cutoff = 0.5;

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float2 uv_T_Pandemic01_N = i.uv_texcoord * _T_Pandemic01_N_ST.xy + _T_Pandemic01_N_ST.zw;
			o.Normal = UnpackNormal( tex2D( _T_Pandemic01_N, uv_T_Pandemic01_N ) );
			float2 uv_T_Pandemic01_BC = i.uv_texcoord * _T_Pandemic01_BC_ST.xy + _T_Pandemic01_BC_ST.zw;
			float4 tex2DNode1 = tex2D( _T_Pandemic01_BC, uv_T_Pandemic01_BC );
			o.Albedo = tex2DNode1.rgb;
			float2 uv_T_Pandemic01_ORM = i.uv_texcoord * _T_Pandemic01_ORM_ST.xy + _T_Pandemic01_ORM_ST.zw;
			float4 tex2DNode2 = tex2D( _T_Pandemic01_ORM, uv_T_Pandemic01_ORM );
			o.Metallic = tex2DNode2.b;
			o.Smoothness = ( 1.0 - tex2DNode2.g );
			o.Occlusion = tex2DNode2.r;
			o.Alpha = 1;
			clip( tex2DNode1.a - _Cutoff );
		}

		ENDCG
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=17101
1990;246;1331;689;1099.27;330.8893;1.3;True;True
Node;AmplifyShaderEditor.SamplerNode;2;-715.934,353.8644;Inherit;True;Property;_T_Pandemic01_ORM;T_Pandemic01_ORM;2;0;Create;True;0;0;False;0;12b4d7139181b834a93df0255e6dd6ce;12b4d7139181b834a93df0255e6dd6ce;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.OneMinusNode;3;-291.5663,379.5954;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;5;-712.8109,150.4815;Inherit;True;Property;_T_Pandemic01_N;T_Pandemic01_N;3;0;Create;True;0;0;False;0;62d0cf01f3fb0274094526f32984b1dc;62d0cf01f3fb0274094526f32984b1dc;True;0;True;bump;Auto;True;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;1;-688.6348,-258.1447;Inherit;True;Property;_T_Pandemic01_BC;T_Pandemic01_BC;1;0;Create;True;0;0;False;0;None;4aac318c38e5e2a4a8dd18fa2e97a486;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;42.53747,-6.380622;Float;False;True;2;ASEMaterialInspector;0;0;Standard;Shader Forge/Character;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Custom;0.5;True;True;0;False;TransparentCutout;;Geometry;All;14;all;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;True;0;0;False;-1;0;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;0;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;3;0;2;2
WireConnection;0;0;1;0
WireConnection;0;1;5;0
WireConnection;0;3;2;3
WireConnection;0;4;3;0
WireConnection;0;5;2;1
WireConnection;0;10;1;4
ASEEND*/
//CHKSM=A47799ADDEF7AADBF294A5528E679678612509B0