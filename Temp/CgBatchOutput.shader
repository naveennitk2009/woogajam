Shader "Toon/Basic" {
	Properties {
		_Color ("Main Color", Color) = (.5,.5,.5,1)
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_ToonShade ("ToonShader Cubemap(RGB)", CUBE) = "" { Texgen CubeNormal }
	}


	SubShader {
		Tags { "RenderType"="Opaque" }
		Pass {
			Name "BASE"
			Cull Off
			
			Program "vp" {
// Vertex combos: 1
//   opengl - ALU: 10 to 10
//   d3d9 - ALU: 10 to 10
//   d3d11 - ALU: 2 to 2, TEX: 0 to 0, FLOW: 1 to 1
//   d3d11_9x - ALU: 2 to 2, TEX: 0 to 0, FLOW: 1 to 1
SubProgram "opengl " {
Keywords { }
Bind "vertex" Vertex
Bind "texcoord" TexCoord0
Bind "normal" Normal
Vector 9 [_MainTex_ST]
"!!ARBvp1.0
# 10 ALU
PARAM c[10] = { { 0 },
		state.matrix.modelview[0],
		state.matrix.mvp,
		program.local[9] };
TEMP R0;
MOV R0.w, c[0].x;
MOV R0.xyz, vertex.normal;
DP4 result.texcoord[1].z, R0, c[3];
DP4 result.texcoord[1].y, R0, c[2];
DP4 result.texcoord[1].x, R0, c[1];
MAD result.texcoord[0].xy, vertex.texcoord[0], c[9], c[9].zwzw;
DP4 result.position.w, vertex.position, c[8];
DP4 result.position.z, vertex.position, c[7];
DP4 result.position.y, vertex.position, c[6];
DP4 result.position.x, vertex.position, c[5];
END
# 10 instructions, 1 R-regs
"
}

SubProgram "d3d9 " {
Keywords { }
Bind "vertex" Vertex
Bind "texcoord" TexCoord0
Bind "normal" Normal
Matrix 0 [glstate_matrix_modelview0]
Matrix 4 [glstate_matrix_mvp]
Vector 8 [_MainTex_ST]
"vs_2_0
; 10 ALU
def c9, 0.00000000, 0, 0, 0
dcl_position0 v0
dcl_texcoord0 v1
dcl_normal0 v2
mov r0.w, c9.x
mov r0.xyz, v2
dp4 oT1.z, r0, c2
dp4 oT1.y, r0, c1
dp4 oT1.x, r0, c0
mad oT0.xy, v1, c8, c8.zwzw
dp4 oPos.w, v0, c7
dp4 oPos.z, v0, c6
dp4 oPos.y, v0, c5
dp4 oPos.x, v0, c4
"
}

SubProgram "d3d11 " {
Keywords { }
Bind "vertex" Vertex
Bind "texcoord" TexCoord0
Bind "normal" Normal
ConstBuffer "$Globals" 48 // 32 used size, 3 vars
Vector 16 [_MainTex_ST] 4
ConstBuffer "UnityPerDraw" 336 // 128 used size, 6 vars
Matrix 0 [glstate_matrix_mvp] 4
Matrix 64 [glstate_matrix_modelview0] 4
BindCB "$Globals" 0
BindCB "UnityPerDraw" 1
// 9 instructions, 1 temp regs, 0 temp arrays:
// ALU 2 float, 0 int, 0 uint
// TEX 0 (0 load, 0 comp, 0 bias, 0 grad)
// FLOW 1 static, 0 dynamic
"vs_4_0
eefiecedjkbjhknghpfggnilhbdnljbhjmimioebabaaaaaammacaaaaadaaaaaa
cmaaaaaakaaaaaaabaabaaaaejfdeheogmaaaaaaadaaaaaaaiaaaaaafaaaaaaa
aaaaaaaaaaaaaaaaadaaaaaaaaaaaaaaapapaaaafjaaaaaaaaaaaaaaaaaaaaaa
adaaaaaaabaaaaaaadadaaaagcaaaaaaaaaaaaaaaaaaaaaaadaaaaaaacaaaaaa
ahahaaaafaepfdejfeejepeoaafeeffiedepepfceeaaeoepfcenebemaaklklkl
epfdeheogiaaaaaaadaaaaaaaiaaaaaafaaaaaaaaaaaaaaaabaaaaaaadaaaaaa
aaaaaaaaapaaaaaafmaaaaaaaaaaaaaaaaaaaaaaadaaaaaaabaaaaaaadamaaaa
fmaaaaaaabaaaaaaaaaaaaaaadaaaaaaacaaaaaaahaiaaaafdfgfpfagphdgjhe
gjgpgoaafeeffiedepepfceeaaklklklfdeieefcleabaaaaeaaaabaagnaaaaaa
fjaaaaaeegiocaaaaaaaaaaaacaaaaaafjaaaaaeegiocaaaabaaaaaaahaaaaaa
fpaaaaadpcbabaaaaaaaaaaafpaaaaaddcbabaaaabaaaaaafpaaaaadhcbabaaa
acaaaaaaghaaaaaepccabaaaaaaaaaaaabaaaaaagfaaaaaddccabaaaabaaaaaa
gfaaaaadhccabaaaacaaaaaagiaaaaacabaaaaaadiaaaaaipcaabaaaaaaaaaaa
fgbfbaaaaaaaaaaaegiocaaaabaaaaaaabaaaaaadcaaaaakpcaabaaaaaaaaaaa
egiocaaaabaaaaaaaaaaaaaaagbabaaaaaaaaaaaegaobaaaaaaaaaaadcaaaaak
pcaabaaaaaaaaaaaegiocaaaabaaaaaaacaaaaaakgbkbaaaaaaaaaaaegaobaaa
aaaaaaaadcaaaaakpccabaaaaaaaaaaaegiocaaaabaaaaaaadaaaaaapgbpbaaa
aaaaaaaaegaobaaaaaaaaaaadcaaaaaldccabaaaabaaaaaaegbabaaaabaaaaaa
egiacaaaaaaaaaaaabaaaaaaogikcaaaaaaaaaaaabaaaaaadiaaaaaihcaabaaa
aaaaaaaafgbfbaaaacaaaaaaegiccaaaabaaaaaaafaaaaaadcaaaaakhcaabaaa
aaaaaaaaegiccaaaabaaaaaaaeaaaaaaagbabaaaacaaaaaaegacbaaaaaaaaaaa
dcaaaaakhccabaaaacaaaaaaegiccaaaabaaaaaaagaaaaaakgbkbaaaacaaaaaa
egacbaaaaaaaaaaadoaaaaab"
}

SubProgram "gles " {
Keywords { }
"!!GLES
#define SHADER_API_GLES 1
#define tex2D texture2D


#ifdef VERTEX
#define gl_ModelViewProjectionMatrix glstate_matrix_mvp
uniform mat4 glstate_matrix_mvp;
#define gl_ModelViewMatrix glstate_matrix_modelview0
uniform mat4 glstate_matrix_modelview0;

varying highp vec3 xlv_TEXCOORD1;
varying highp vec2 xlv_TEXCOORD0;
uniform highp vec4 _MainTex_ST;


attribute vec4 _glesMultiTexCoord0;
attribute vec3 _glesNormal;
attribute vec4 _glesVertex;
void main ()
{
  highp vec4 tmpvar_1;
  tmpvar_1.w = 0.0;
  tmpvar_1.xyz = normalize(_glesNormal);
  gl_Position = (gl_ModelViewProjectionMatrix * _glesVertex);
  xlv_TEXCOORD0 = ((_glesMultiTexCoord0.xy * _MainTex_ST.xy) + _MainTex_ST.zw);
  xlv_TEXCOORD1 = (gl_ModelViewMatrix * tmpvar_1).xyz;
}



#endif
#ifdef FRAGMENT

varying highp vec3 xlv_TEXCOORD1;
varying highp vec2 xlv_TEXCOORD0;
uniform highp vec4 _Color;
uniform samplerCube _ToonShade;
uniform sampler2D _MainTex;
void main ()
{
  highp vec4 cube_1;
  lowp vec4 tmpvar_2;
  tmpvar_2 = texture2D (_MainTex, xlv_TEXCOORD0);
  highp vec4 tmpvar_3;
  tmpvar_3 = (_Color * tmpvar_2);
  lowp vec4 tmpvar_4;
  tmpvar_4 = textureCube (_ToonShade, xlv_TEXCOORD1);
  cube_1 = tmpvar_4;
  highp vec4 tmpvar_5;
  tmpvar_5.xyz = ((2.0 * cube_1.xyz) * tmpvar_3.xyz);
  tmpvar_5.w = tmpvar_3.w;
  gl_FragData[0] = tmpvar_5;
}



#endif"
}

SubProgram "glesdesktop " {
Keywords { }
"!!GLES
#define SHADER_API_GLES 1
#define tex2D texture2D


#ifdef VERTEX
#define gl_ModelViewProjectionMatrix glstate_matrix_mvp
uniform mat4 glstate_matrix_mvp;
#define gl_ModelViewMatrix glstate_matrix_modelview0
uniform mat4 glstate_matrix_modelview0;

varying highp vec3 xlv_TEXCOORD1;
varying highp vec2 xlv_TEXCOORD0;
uniform highp vec4 _MainTex_ST;


attribute vec4 _glesMultiTexCoord0;
attribute vec3 _glesNormal;
attribute vec4 _glesVertex;
void main ()
{
  highp vec4 tmpvar_1;
  tmpvar_1.w = 0.0;
  tmpvar_1.xyz = normalize(_glesNormal);
  gl_Position = (gl_ModelViewProjectionMatrix * _glesVertex);
  xlv_TEXCOORD0 = ((_glesMultiTexCoord0.xy * _MainTex_ST.xy) + _MainTex_ST.zw);
  xlv_TEXCOORD1 = (gl_ModelViewMatrix * tmpvar_1).xyz;
}



#endif
#ifdef FRAGMENT

varying highp vec3 xlv_TEXCOORD1;
varying highp vec2 xlv_TEXCOORD0;
uniform highp vec4 _Color;
uniform samplerCube _ToonShade;
uniform sampler2D _MainTex;
void main ()
{
  highp vec4 cube_1;
  lowp vec4 tmpvar_2;
  tmpvar_2 = texture2D (_MainTex, xlv_TEXCOORD0);
  highp vec4 tmpvar_3;
  tmpvar_3 = (_Color * tmpvar_2);
  lowp vec4 tmpvar_4;
  tmpvar_4 = textureCube (_ToonShade, xlv_TEXCOORD1);
  cube_1 = tmpvar_4;
  highp vec4 tmpvar_5;
  tmpvar_5.xyz = ((2.0 * cube_1.xyz) * tmpvar_3.xyz);
  tmpvar_5.w = tmpvar_3.w;
  gl_FragData[0] = tmpvar_5;
}



#endif"
}

SubProgram "flash " {
Keywords { }
Bind "vertex" Vertex
Bind "texcoord" TexCoord0
Bind "normal" Normal
Matrix 0 [glstate_matrix_modelview0]
Matrix 4 [glstate_matrix_mvp]
Vector 8 [_MainTex_ST]
"agal_vs
c9 0.0 0.0 0.0 0.0
[bc]
aaaaaaaaaaaaaiacajaaaaaaabaaaaaaaaaaaaaaaaaaaaaa mov r0.w, c9.x
aaaaaaaaaaaaahacabaaaaoeaaaaaaaaaaaaaaaaaaaaaaaa mov r0.xyz, a1
bdaaaaaaabaaaeaeaaaaaaoeacaaaaaaacaaaaoeabaaaaaa dp4 v1.z, r0, c2
bdaaaaaaabaaacaeaaaaaaoeacaaaaaaabaaaaoeabaaaaaa dp4 v1.y, r0, c1
bdaaaaaaabaaabaeaaaaaaoeacaaaaaaaaaaaaoeabaaaaaa dp4 v1.x, r0, c0
adaaaaaaaaaaadacadaaaaoeaaaaaaaaaiaaaaoeabaaaaaa mul r0.xy, a3, c8
abaaaaaaaaaaadaeaaaaaafeacaaaaaaaiaaaaooabaaaaaa add v0.xy, r0.xyyy, c8.zwzw
bdaaaaaaaaaaaiadaaaaaaoeaaaaaaaaahaaaaoeabaaaaaa dp4 o0.w, a0, c7
bdaaaaaaaaaaaeadaaaaaaoeaaaaaaaaagaaaaoeabaaaaaa dp4 o0.z, a0, c6
bdaaaaaaaaaaacadaaaaaaoeaaaaaaaaafaaaaoeabaaaaaa dp4 o0.y, a0, c5
bdaaaaaaaaaaabadaaaaaaoeaaaaaaaaaeaaaaoeabaaaaaa dp4 o0.x, a0, c4
aaaaaaaaaaaaamaeaaaaaaoeabaaaaaaaaaaaaaaaaaaaaaa mov v0.zw, c0
aaaaaaaaabaaaiaeaaaaaaoeabaaaaaaaaaaaaaaaaaaaaaa mov v1.w, c0
"
}

SubProgram "d3d11_9x " {
Keywords { }
Bind "vertex" Vertex
Bind "texcoord" TexCoord0
Bind "normal" Normal
ConstBuffer "$Globals" 48 // 32 used size, 3 vars
Vector 16 [_MainTex_ST] 4
ConstBuffer "UnityPerDraw" 336 // 128 used size, 6 vars
Matrix 0 [glstate_matrix_mvp] 4
Matrix 64 [glstate_matrix_modelview0] 4
BindCB "$Globals" 0
BindCB "UnityPerDraw" 1
// 9 instructions, 1 temp regs, 0 temp arrays:
// ALU 2 float, 0 int, 0 uint
// TEX 0 (0 load, 0 comp, 0 bias, 0 grad)
// FLOW 1 static, 0 dynamic
"vs_4_0_level_9_3
eefiecednmjlkdojgagiiolnakcmdmohhdlmmenjabaaaaaapmadaaaaaeaaaaaa
daaaaaaafmabaaaabiadaaaaimadaaaaebgpgodjceabaaaaceabaaaaaaacpopp
oeaaaaaaeaaaaaaaacaaceaaaaaadmaaaaaadmaaaaaaceaaabaadmaaaaaaabaa
abaaabaaaaaaaaaaabaaaaaaahaaacaaaaaaaaaaaaaaaaaaabacpoppbpaaaaac
afaaaaiaaaaaapjabpaaaaacafaaabiaabaaapjabpaaaaacafaaaciaacaaapja
aeaaaaaeaaaaadoaabaaoejaabaaoekaabaaookaafaaaaadaaaaahiaacaaffja
ahaaoekaaeaaaaaeaaaaahiaagaaoekaacaaaajaaaaaoeiaaeaaaaaeabaaahoa
aiaaoekaacaakkjaaaaaoeiaafaaaaadaaaaapiaaaaaffjaadaaoekaaeaaaaae
aaaaapiaacaaoekaaaaaaajaaaaaoeiaaeaaaaaeaaaaapiaaeaaoekaaaaakkja
aaaaoeiaaeaaaaaeaaaaapiaafaaoekaaaaappjaaaaaoeiaaeaaaaaeaaaaadma
aaaappiaaaaaoekaaaaaoeiaabaaaaacaaaaammaaaaaoeiappppaaaafdeieefc
leabaaaaeaaaabaagnaaaaaafjaaaaaeegiocaaaaaaaaaaaacaaaaaafjaaaaae
egiocaaaabaaaaaaahaaaaaafpaaaaadpcbabaaaaaaaaaaafpaaaaaddcbabaaa
abaaaaaafpaaaaadhcbabaaaacaaaaaaghaaaaaepccabaaaaaaaaaaaabaaaaaa
gfaaaaaddccabaaaabaaaaaagfaaaaadhccabaaaacaaaaaagiaaaaacabaaaaaa
diaaaaaipcaabaaaaaaaaaaafgbfbaaaaaaaaaaaegiocaaaabaaaaaaabaaaaaa
dcaaaaakpcaabaaaaaaaaaaaegiocaaaabaaaaaaaaaaaaaaagbabaaaaaaaaaaa
egaobaaaaaaaaaaadcaaaaakpcaabaaaaaaaaaaaegiocaaaabaaaaaaacaaaaaa
kgbkbaaaaaaaaaaaegaobaaaaaaaaaaadcaaaaakpccabaaaaaaaaaaaegiocaaa
abaaaaaaadaaaaaapgbpbaaaaaaaaaaaegaobaaaaaaaaaaadcaaaaaldccabaaa
abaaaaaaegbabaaaabaaaaaaegiacaaaaaaaaaaaabaaaaaaogikcaaaaaaaaaaa
abaaaaaadiaaaaaihcaabaaaaaaaaaaafgbfbaaaacaaaaaaegiccaaaabaaaaaa
afaaaaaadcaaaaakhcaabaaaaaaaaaaaegiccaaaabaaaaaaaeaaaaaaagbabaaa
acaaaaaaegacbaaaaaaaaaaadcaaaaakhccabaaaacaaaaaaegiccaaaabaaaaaa
agaaaaaakgbkbaaaacaaaaaaegacbaaaaaaaaaaadoaaaaabejfdeheogmaaaaaa
adaaaaaaaiaaaaaafaaaaaaaaaaaaaaaaaaaaaaaadaaaaaaaaaaaaaaapapaaaa
fjaaaaaaaaaaaaaaaaaaaaaaadaaaaaaabaaaaaaadadaaaagcaaaaaaaaaaaaaa
aaaaaaaaadaaaaaaacaaaaaaahahaaaafaepfdejfeejepeoaafeeffiedepepfc
eeaaeoepfcenebemaaklklklepfdeheogiaaaaaaadaaaaaaaiaaaaaafaaaaaaa
aaaaaaaaabaaaaaaadaaaaaaaaaaaaaaapaaaaaafmaaaaaaaaaaaaaaaaaaaaaa
adaaaaaaabaaaaaaadamaaaafmaaaaaaabaaaaaaaaaaaaaaadaaaaaaacaaaaaa
ahaiaaaafdfgfpfagphdgjhegjgpgoaafeeffiedepepfceeaaklklkl"
}

}
Program "fp" {
// Fragment combos: 1
//   opengl - ALU: 6 to 6, TEX: 2 to 2
//   d3d9 - ALU: 5 to 5, TEX: 2 to 2
//   d3d11 - ALU: 3 to 3, TEX: 2 to 2, FLOW: 1 to 1
//   d3d11_9x - ALU: 3 to 3, TEX: 2 to 2, FLOW: 1 to 1
SubProgram "opengl " {
Keywords { }
Vector 0 [_Color]
SetTexture 0 [_MainTex] 2D
SetTexture 1 [_ToonShade] CUBE
"!!ARBfp1.0
OPTION ARB_precision_hint_fastest;
# 6 ALU, 2 TEX
PARAM c[2] = { program.local[0],
		{ 2 } };
TEMP R0;
TEMP R1;
TEX R0, fragment.texcoord[0], texture[0], 2D;
TEX R1.xyz, fragment.texcoord[1], texture[1], CUBE;
MUL R0, R0, c[0];
MUL R0.xyz, R1, R0;
MUL result.color.xyz, R0, c[1].x;
MOV result.color.w, R0;
END
# 6 instructions, 2 R-regs
"
}

SubProgram "d3d9 " {
Keywords { }
Vector 0 [_Color]
SetTexture 0 [_MainTex] 2D
SetTexture 1 [_ToonShade] CUBE
"ps_2_0
; 5 ALU, 2 TEX
dcl_2d s0
dcl_cube s1
def c1, 2.00000000, 0, 0, 0
dcl t0.xy
dcl t1.xyz
texld r0, t1, s1
texld r1, t0, s0
mul r1, r1, c0
mul r0.xyz, r0, r1
mov r0.w, r1
mul r0.xyz, r0, c1.x
mov oC0, r0
"
}

SubProgram "d3d11 " {
Keywords { }
ConstBuffer "$Globals" 48 // 48 used size, 3 vars
Vector 32 [_Color] 4
BindCB "$Globals" 0
SetTexture 0 [_MainTex] 2D 0
SetTexture 1 [_ToonShade] CUBE 1
// 7 instructions, 2 temp regs, 0 temp arrays:
// ALU 3 float, 0 int, 0 uint
// TEX 2 (0 load, 0 comp, 0 bias, 0 grad)
// FLOW 1 static, 0 dynamic
"ps_4_0
eefieceddpgpgncipggmopcjcpenfjikhihbhflbabaaaaaaamacaaaaadaaaaaa
cmaaaaaajmaaaaaanaaaaaaaejfdeheogiaaaaaaadaaaaaaaiaaaaaafaaaaaaa
aaaaaaaaabaaaaaaadaaaaaaaaaaaaaaapaaaaaafmaaaaaaaaaaaaaaaaaaaaaa
adaaaaaaabaaaaaaadadaaaafmaaaaaaabaaaaaaaaaaaaaaadaaaaaaacaaaaaa
ahahaaaafdfgfpfagphdgjhegjgpgoaafeeffiedepepfceeaaklklklepfdeheo
cmaaaaaaabaaaaaaaiaaaaaacaaaaaaaaaaaaaaaaaaaaaaaadaaaaaaaaaaaaaa
apaaaaaafdfgfpfegbhcghgfheaaklklfdeieefcdeabaaaaeaaaaaaaenaaaaaa
fjaaaaaeegiocaaaaaaaaaaaadaaaaaafkaaaaadaagabaaaaaaaaaaafkaaaaad
aagabaaaabaaaaaafibiaaaeaahabaaaaaaaaaaaffffaaaafidaaaaeaahabaaa
abaaaaaaffffaaaagcbaaaaddcbabaaaabaaaaaagcbaaaadhcbabaaaacaaaaaa
gfaaaaadpccabaaaaaaaaaaagiaaaaacacaaaaaaefaaaaajpcaabaaaaaaaaaaa
egbcbaaaacaaaaaaeghobaaaabaaaaaaaagabaaaabaaaaaaefaaaaajpcaabaaa
abaaaaaaegbabaaaabaaaaaaeghobaaaaaaaaaaaaagabaaaaaaaaaaadiaaaaai
pcaabaaaabaaaaaaegaobaaaabaaaaaaegiocaaaaaaaaaaaacaaaaaadiaaaaah
hcaabaaaaaaaaaaaegacbaaaaaaaaaaaegacbaaaabaaaaaadgaaaaaficcabaaa
aaaaaaaadkaabaaaabaaaaaaaaaaaaahhccabaaaaaaaaaaaegacbaaaaaaaaaaa
egacbaaaaaaaaaaadoaaaaab"
}

SubProgram "gles " {
Keywords { }
"!!GLES"
}

SubProgram "glesdesktop " {
Keywords { }
"!!GLES"
}

SubProgram "flash " {
Keywords { }
Vector 0 [_Color]
SetTexture 0 [_MainTex] 2D
SetTexture 1 [_ToonShade] CUBE
"agal_ps
c1 2.0 0.0 0.0 0.0
[bc]
ciaaaaaaaaaaapacabaaaaoeaeaaaaaaabaaaaaaafbababb tex r0, v1, s1 <cube wrap linear point>
ciaaaaaaabaaapacaaaaaaoeaeaaaaaaaaaaaaaaafaababb tex r1, v0, s0 <2d wrap linear point>
adaaaaaaabaaapacabaaaaoeacaaaaaaaaaaaaoeabaaaaaa mul r1, r1, c0
adaaaaaaaaaaahacaaaaaakeacaaaaaaabaaaakeacaaaaaa mul r0.xyz, r0.xyzz, r1.xyzz
aaaaaaaaaaaaaiacabaaaappacaaaaaaaaaaaaaaaaaaaaaa mov r0.w, r1.w
adaaaaaaaaaaahacaaaaaakeacaaaaaaabaaaaaaabaaaaaa mul r0.xyz, r0.xyzz, c1.x
aaaaaaaaaaaaapadaaaaaaoeacaaaaaaaaaaaaaaaaaaaaaa mov o0, r0
"
}

SubProgram "d3d11_9x " {
Keywords { }
ConstBuffer "$Globals" 48 // 48 used size, 3 vars
Vector 32 [_Color] 4
BindCB "$Globals" 0
SetTexture 0 [_MainTex] 2D 0
SetTexture 1 [_ToonShade] CUBE 1
// 7 instructions, 2 temp regs, 0 temp arrays:
// ALU 3 float, 0 int, 0 uint
// TEX 2 (0 load, 0 comp, 0 bias, 0 grad)
// FLOW 1 static, 0 dynamic
"ps_4_0_level_9_3
eefiecedehdfmcelmmheddiaoclhhdlobfdlmalaabaaaaaaoeacaaaaaeaaaaaa
daaaaaaaaeabaaaaeaacaaaalaacaaaaebgpgodjmmaaaaaammaaaaaaaaacpppp
jeaaaaaadiaaaaaaabaacmaaaaaadiaaaaaadiaaacaaceaaaaaadiaaaaaaaaaa
abababaaaaaaacaaabaaaaaaaaaaaaaaabacppppbpaaaaacaaaaaaiaaaaaadla
bpaaaaacaaaaaaiaabaaahlabpaaaaacaaaaaajaaaaiapkabpaaaaacaaaaaaji
abaiapkaecaaaaadaaaaapiaaaaaoelaaaaioekaecaaaaadabaaapiaabaaoela
abaioekaafaaaaadaaaaapiaaaaaoeiaaaaaoekaafaaaaadabaaahiaabaaoeia
aaaaoeiaacaaaaadaaaaahiaabaaoeiaabaaoeiaabaaaaacaaaiapiaaaaaoeia
ppppaaaafdeieefcdeabaaaaeaaaaaaaenaaaaaafjaaaaaeegiocaaaaaaaaaaa
adaaaaaafkaaaaadaagabaaaaaaaaaaafkaaaaadaagabaaaabaaaaaafibiaaae
aahabaaaaaaaaaaaffffaaaafidaaaaeaahabaaaabaaaaaaffffaaaagcbaaaad
dcbabaaaabaaaaaagcbaaaadhcbabaaaacaaaaaagfaaaaadpccabaaaaaaaaaaa
giaaaaacacaaaaaaefaaaaajpcaabaaaaaaaaaaaegbcbaaaacaaaaaaeghobaaa
abaaaaaaaagabaaaabaaaaaaefaaaaajpcaabaaaabaaaaaaegbabaaaabaaaaaa
eghobaaaaaaaaaaaaagabaaaaaaaaaaadiaaaaaipcaabaaaabaaaaaaegaobaaa
abaaaaaaegiocaaaaaaaaaaaacaaaaaadiaaaaahhcaabaaaaaaaaaaaegacbaaa
aaaaaaaaegacbaaaabaaaaaadgaaaaaficcabaaaaaaaaaaadkaabaaaabaaaaaa
aaaaaaahhccabaaaaaaaaaaaegacbaaaaaaaaaaaegacbaaaaaaaaaaadoaaaaab
ejfdeheogiaaaaaaadaaaaaaaiaaaaaafaaaaaaaaaaaaaaaabaaaaaaadaaaaaa
aaaaaaaaapaaaaaafmaaaaaaaaaaaaaaaaaaaaaaadaaaaaaabaaaaaaadadaaaa
fmaaaaaaabaaaaaaaaaaaaaaadaaaaaaacaaaaaaahahaaaafdfgfpfagphdgjhe
gjgpgoaafeeffiedepepfceeaaklklklepfdeheocmaaaaaaabaaaaaaaiaaaaaa
caaaaaaaaaaaaaaaaaaaaaaaadaaaaaaaaaaaaaaapaaaaaafdfgfpfegbhcghgf
heaaklkl"
}

}

#LINE 54
			
		}
	} 

	SubShader {
		Tags { "RenderType"="Opaque" }
		Pass {
			Name "BASE"
			Cull Off
			SetTexture [_MainTex] {
				constantColor [_Color]
				Combine texture * constant
			} 
			SetTexture [_ToonShade] {
				combine texture * previous DOUBLE, previous
			}
		}
	} 
	
	Fallback "VertexLit"
}
