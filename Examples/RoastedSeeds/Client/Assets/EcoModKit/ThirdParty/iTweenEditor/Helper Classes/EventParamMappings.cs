// Copyright (c) 2009 David Koontz
// Please direct any bugs/comments/suggestions to david@koontzfamily.org
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

using System;
using System.Collections.Generic;
using UnityEngine;

public class EventParamMappings {
		
	public static Dictionary<iTweenEvent.TweenType, Dictionary<string, Type>> mappings = new Dictionary<iTweenEvent.TweenType, Dictionary<string, Type>>();
	
	static EventParamMappings() {
		// AUDIO FROM
		mappings.Add(iTweenEvent.TweenType.AudioFrom, new Dictionary<string, Type>());
		mappings[iTweenEvent.TweenType.AudioFrom]["audiosource"] = typeof(AudioSource);
		mappings[iTweenEvent.TweenType.AudioFrom]["volume"] = typeof(float);
		mappings[iTweenEvent.TweenType.AudioFrom]["pitch"] = typeof(float);
		mappings[iTweenEvent.TweenType.AudioFrom]["time"] = typeof(float);
		mappings[iTweenEvent.TweenType.AudioFrom]["delay"] = typeof(float);
		mappings[iTweenEvent.TweenType.AudioFrom]["easetype"] = typeof(iTween.EaseType);
		mappings[iTweenEvent.TweenType.AudioFrom]["looptype"] = typeof(iTween.LoopType);
		mappings[iTweenEvent.TweenType.AudioFrom]["onstart"] = typeof(string);
		mappings[iTweenEvent.TweenType.AudioFrom]["onstarttarget"] = typeof(GameObject);
		mappings[iTweenEvent.TweenType.AudioFrom]["onstartparams"] = typeof(string);
		mappings[iTweenEvent.TweenType.AudioFrom]["onupdate"] = typeof(string);
		mappings[iTweenEvent.TweenType.AudioFrom]["onupdatetarget"] = typeof(GameObject);
		mappings[iTweenEvent.TweenType.AudioFrom]["onupdateparams"] = typeof(string);
		mappings[iTweenEvent.TweenType.AudioFrom]["oncomplete"] = typeof(string);
		mappings[iTweenEvent.TweenType.AudioFrom]["oncompletetarget"] = typeof(GameObject);
		mappings[iTweenEvent.TweenType.AudioFrom]["oncompleteparams"] = typeof(string);
		mappings[iTweenEvent.TweenType.AudioFrom]["ignoretimescale"] = typeof(bool);
		
		// AUDIO TO
		mappings.Add(iTweenEvent.TweenType.AudioTo, new Dictionary<string, Type>());
		mappings[iTweenEvent.TweenType.AudioTo]["audiosource"] = typeof(AudioSource);
		mappings[iTweenEvent.TweenType.AudioTo]["volume"] = typeof(float);
		mappings[iTweenEvent.TweenType.AudioTo]["pitch"] = typeof(float);
		mappings[iTweenEvent.TweenType.AudioTo]["time"] = typeof(float);
		mappings[iTweenEvent.TweenType.AudioTo]["delay"] = typeof(float);
		mappings[iTweenEvent.TweenType.AudioTo]["easetype"] = typeof(iTween.EaseType);
		mappings[iTweenEvent.TweenType.AudioTo]["looptype"] = typeof(iTween.LoopType);
		mappings[iTweenEvent.TweenType.AudioTo]["onstart"] = typeof(string);
		mappings[iTweenEvent.TweenType.AudioTo]["onstarttarget"] = typeof(GameObject);
		mappings[iTweenEvent.TweenType.AudioTo]["onstartparams"] = typeof(string);
		mappings[iTweenEvent.TweenType.AudioTo]["onupdate"] = typeof(string);
		mappings[iTweenEvent.TweenType.AudioTo]["onupdatetarget"] = typeof(GameObject);
		mappings[iTweenEvent.TweenType.AudioTo]["onupdateparams"] = typeof(string);
		mappings[iTweenEvent.TweenType.AudioTo]["oncomplete"] = typeof(string);
		mappings[iTweenEvent.TweenType.AudioTo]["oncompletetarget"] = typeof(GameObject);
		mappings[iTweenEvent.TweenType.AudioTo]["oncompleteparams"] = typeof(string);
		mappings[iTweenEvent.TweenType.AudioTo]["ignoretimescale"] = typeof(bool);
		
		// AUDIO UPDATE
		mappings.Add(iTweenEvent.TweenType.AudioUpdate, new Dictionary<string, Type>());
		mappings[iTweenEvent.TweenType.AudioUpdate]["audiosource"] = typeof(AudioSource);
		mappings[iTweenEvent.TweenType.AudioUpdate]["volume"] = typeof(float);
		mappings[iTweenEvent.TweenType.AudioUpdate]["pitch"] = typeof(float);
		mappings[iTweenEvent.TweenType.AudioUpdate]["time"] = typeof(float);
		
		// CAMERA FADE FROM
		mappings.Add(iTweenEvent.TweenType.CameraFadeFrom, new Dictionary<string, Type>());
		mappings[iTweenEvent.TweenType.CameraFadeFrom]["amount"] = typeof(float);
		mappings[iTweenEvent.TweenType.CameraFadeFrom]["time"] = typeof(float);
		mappings[iTweenEvent.TweenType.CameraFadeFrom]["delay"] = typeof(float);
		mappings[iTweenEvent.TweenType.CameraFadeFrom]["easetype"] = typeof(iTween.EaseType);
		mappings[iTweenEvent.TweenType.CameraFadeFrom]["looptype"] = typeof(iTween.LoopType);
		mappings[iTweenEvent.TweenType.CameraFadeFrom]["onstart"] = typeof(string);
		mappings[iTweenEvent.TweenType.CameraFadeFrom]["onstarttarget"] = typeof(GameObject);
		mappings[iTweenEvent.TweenType.CameraFadeFrom]["onstartparams"] = typeof(string);
		mappings[iTweenEvent.TweenType.CameraFadeFrom]["onupdate"] = typeof(string);
		mappings[iTweenEvent.TweenType.CameraFadeFrom]["onupdatetarget"] = typeof(GameObject);
		mappings[iTweenEvent.TweenType.CameraFadeFrom]["onupdateparams"] = typeof(string);
		mappings[iTweenEvent.TweenType.CameraFadeFrom]["oncomplete"] = typeof(string);
		mappings[iTweenEvent.TweenType.CameraFadeFrom]["oncompletetarget"] = typeof(GameObject);
		mappings[iTweenEvent.TweenType.CameraFadeFrom]["oncompleteparams"] = typeof(string);
		mappings[iTweenEvent.TweenType.CameraFadeFrom]["ignoretimescale"] = typeof(bool);
		
		// CAMERA FADE TO
		mappings.Add(iTweenEvent.TweenType.CameraFadeTo, new Dictionary<string, Type>());
		mappings[iTweenEvent.TweenType.CameraFadeTo]["amount"] = typeof(float);
		mappings[iTweenEvent.TweenType.CameraFadeTo]["time"] = typeof(float);
		mappings[iTweenEvent.TweenType.CameraFadeTo]["delay"] = typeof(float);
		mappings[iTweenEvent.TweenType.CameraFadeTo]["easetype"] = typeof(iTween.EaseType);
		mappings[iTweenEvent.TweenType.CameraFadeTo]["looptype"] = typeof(iTween.LoopType);
		mappings[iTweenEvent.TweenType.CameraFadeTo]["onstart"] = typeof(string);
		mappings[iTweenEvent.TweenType.CameraFadeTo]["onstarttarget"] = typeof(GameObject);
		mappings[iTweenEvent.TweenType.CameraFadeTo]["onstartparams"] = typeof(string);
		mappings[iTweenEvent.TweenType.CameraFadeTo]["onupdate"] = typeof(string);
		mappings[iTweenEvent.TweenType.CameraFadeTo]["onupdatetarget"] = typeof(GameObject);
		mappings[iTweenEvent.TweenType.CameraFadeTo]["onupdateparams"] = typeof(string);
		mappings[iTweenEvent.TweenType.CameraFadeTo]["oncomplete"] = typeof(string);
		mappings[iTweenEvent.TweenType.CameraFadeTo]["oncompletetarget"] = typeof(GameObject);
		mappings[iTweenEvent.TweenType.CameraFadeTo]["oncompleteparams"] = typeof(string);
		mappings[iTweenEvent.TweenType.CameraFadeTo]["ignoretimescale"] = typeof(bool);
		
		// COLOR FROM
		mappings.Add(iTweenEvent.TweenType.ColorFrom, new Dictionary<string, Type>());
		mappings[iTweenEvent.TweenType.ColorFrom]["color"] = typeof(Color);
		mappings[iTweenEvent.TweenType.ColorFrom]["r"] = typeof(float);
		mappings[iTweenEvent.TweenType.ColorFrom]["g"] = typeof(float);
		mappings[iTweenEvent.TweenType.ColorFrom]["b"] = typeof(float);
		mappings[iTweenEvent.TweenType.ColorFrom]["a"] = typeof(float);
		mappings[iTweenEvent.TweenType.ColorFrom]["namedcolorvalue"] = typeof(string);
		mappings[iTweenEvent.TweenType.ColorFrom]["includechildren"] = typeof(bool);
		mappings[iTweenEvent.TweenType.ColorFrom]["time"] = typeof(float);
		mappings[iTweenEvent.TweenType.ColorFrom]["delay"] = typeof(float);
		mappings[iTweenEvent.TweenType.ColorFrom]["easetype"] = typeof(iTween.EaseType);
		mappings[iTweenEvent.TweenType.ColorFrom]["looptype"] = typeof(iTween.LoopType);
		mappings[iTweenEvent.TweenType.ColorFrom]["onstart"] = typeof(string);
		mappings[iTweenEvent.TweenType.ColorFrom]["onstarttarget"] = typeof(GameObject);
		mappings[iTweenEvent.TweenType.ColorFrom]["onstartparams"] = typeof(string);
		mappings[iTweenEvent.TweenType.ColorFrom]["onupdate"] = typeof(string);
		mappings[iTweenEvent.TweenType.ColorFrom]["onupdatetarget"] = typeof(GameObject);
		mappings[iTweenEvent.TweenType.ColorFrom]["onupdateparams"] = typeof(string);
		mappings[iTweenEvent.TweenType.ColorFrom]["oncomplete"] = typeof(string);
		mappings[iTweenEvent.TweenType.ColorFrom]["oncompletetarget"] = typeof(GameObject);
		mappings[iTweenEvent.TweenType.ColorFrom]["oncompleteparams"] = typeof(string);
		mappings[iTweenEvent.TweenType.ColorFrom]["ignoretimescale"] = typeof(bool);
		
		// COLOR TO
		mappings.Add(iTweenEvent.TweenType.ColorTo, new Dictionary<string, Type>());
		mappings[iTweenEvent.TweenType.ColorTo]["color"] = typeof(Color);
		mappings[iTweenEvent.TweenType.ColorTo]["r"] = typeof(float);
		mappings[iTweenEvent.TweenType.ColorTo]["g"] = typeof(float);
		mappings[iTweenEvent.TweenType.ColorTo]["b"] = typeof(float);
		mappings[iTweenEvent.TweenType.ColorTo]["a"] = typeof(float);
		mappings[iTweenEvent.TweenType.ColorTo]["namedcolorvalue"] = typeof(string);
		mappings[iTweenEvent.TweenType.ColorTo]["includechildren"] = typeof(bool);
		mappings[iTweenEvent.TweenType.ColorTo]["time"] = typeof(float);
		mappings[iTweenEvent.TweenType.ColorTo]["delay"] = typeof(float);
		mappings[iTweenEvent.TweenType.ColorTo]["easetype"] = typeof(iTween.EaseType);
		mappings[iTweenEvent.TweenType.ColorTo]["looptype"] = typeof(iTween.LoopType);
		mappings[iTweenEvent.TweenType.ColorTo]["onstart"] = typeof(string);
		mappings[iTweenEvent.TweenType.ColorTo]["onstarttarget"] = typeof(GameObject);
		mappings[iTweenEvent.TweenType.ColorTo]["onstartparams"] = typeof(string);
		mappings[iTweenEvent.TweenType.ColorTo]["onupdate"] = typeof(string);
		mappings[iTweenEvent.TweenType.ColorTo]["onupdatetarget"] = typeof(GameObject);
		mappings[iTweenEvent.TweenType.ColorTo]["onupdateparams"] = typeof(string);
		mappings[iTweenEvent.TweenType.ColorTo]["oncomplete"] = typeof(string);
		mappings[iTweenEvent.TweenType.ColorTo]["oncompletetarget"] = typeof(GameObject);
		mappings[iTweenEvent.TweenType.ColorTo]["oncompleteparams"] = typeof(string);
		mappings[iTweenEvent.TweenType.ColorTo]["ignoretimescale"] = typeof(bool);
		
		// COLOR UPDATE
		mappings.Add(iTweenEvent.TweenType.ColorUpdate, new Dictionary<string, Type>());
		mappings[iTweenEvent.TweenType.ColorUpdate]["color"] = typeof(Color);
		mappings[iTweenEvent.TweenType.ColorUpdate]["r"] = typeof(float);
		mappings[iTweenEvent.TweenType.ColorUpdate]["g"] = typeof(float);
		mappings[iTweenEvent.TweenType.ColorUpdate]["b"] = typeof(float);
		mappings[iTweenEvent.TweenType.ColorUpdate]["a"] = typeof(float);
		mappings[iTweenEvent.TweenType.ColorUpdate]["namedcolorvalue"] = typeof(string);
		mappings[iTweenEvent.TweenType.ColorUpdate]["includechildren"] = typeof(bool);
		mappings[iTweenEvent.TweenType.ColorUpdate]["time"] = typeof(float);
		
		// FADE FROM
		mappings.Add(iTweenEvent.TweenType.FadeFrom, new Dictionary<string, Type>());
		mappings[iTweenEvent.TweenType.FadeFrom]["alpha"] = typeof(float);
		mappings[iTweenEvent.TweenType.FadeFrom]["amount"] = typeof(float);
		mappings[iTweenEvent.TweenType.FadeFrom]["includechildren"] = typeof(bool);
		mappings[iTweenEvent.TweenType.FadeFrom]["time"] = typeof(float);
		mappings[iTweenEvent.TweenType.FadeFrom]["delay"] = typeof(float);
		mappings[iTweenEvent.TweenType.FadeFrom]["easetype"] = typeof(iTween.EaseType);
		mappings[iTweenEvent.TweenType.FadeFrom]["looptype"] = typeof(iTween.LoopType);
		mappings[iTweenEvent.TweenType.FadeFrom]["onstart"] = typeof(string);
		mappings[iTweenEvent.TweenType.FadeFrom]["onstarttarget"] = typeof(GameObject);
		mappings[iTweenEvent.TweenType.FadeFrom]["onstartparams"] = typeof(string);
		mappings[iTweenEvent.TweenType.FadeFrom]["onupdate"] = typeof(string);
		mappings[iTweenEvent.TweenType.FadeFrom]["onupdatetarget"] = typeof(GameObject);
		mappings[iTweenEvent.TweenType.FadeFrom]["onupdateparams"] = typeof(string);
		mappings[iTweenEvent.TweenType.FadeFrom]["oncomplete"] = typeof(string);
		mappings[iTweenEvent.TweenType.FadeFrom]["oncompletetarget"] = typeof(GameObject);
		mappings[iTweenEvent.TweenType.FadeFrom]["oncompleteparams"] = typeof(string);
		mappings[iTweenEvent.TweenType.FadeFrom]["ignoretimescale"] = typeof(bool);
		
		// FADE TO
		mappings.Add(iTweenEvent.TweenType.FadeTo, new Dictionary<string, Type>());
		mappings[iTweenEvent.TweenType.FadeTo]["alpha"] = typeof(float);
		mappings[iTweenEvent.TweenType.FadeTo]["amount"] = typeof(float);
		mappings[iTweenEvent.TweenType.FadeTo]["includechildren"] = typeof(bool);
		mappings[iTweenEvent.TweenType.FadeTo]["time"] = typeof(float);
		mappings[iTweenEvent.TweenType.FadeTo]["delay"] = typeof(float);
		mappings[iTweenEvent.TweenType.FadeTo]["easetype"] = typeof(iTween.EaseType);
		mappings[iTweenEvent.TweenType.FadeTo]["looptype"] = typeof(iTween.LoopType);
		mappings[iTweenEvent.TweenType.FadeTo]["onstart"] = typeof(string);
		mappings[iTweenEvent.TweenType.FadeTo]["onstarttarget"] = typeof(GameObject);
		mappings[iTweenEvent.TweenType.FadeTo]["onstartparams"] = typeof(string);
		mappings[iTweenEvent.TweenType.FadeTo]["onupdate"] = typeof(string);
		mappings[iTweenEvent.TweenType.FadeTo]["onupdatetarget"] = typeof(GameObject);
		mappings[iTweenEvent.TweenType.FadeTo]["onupdateparams"] = typeof(string);
		mappings[iTweenEvent.TweenType.FadeTo]["oncomplete"] = typeof(string);
		mappings[iTweenEvent.TweenType.FadeTo]["oncompletetarget"] = typeof(GameObject);
		mappings[iTweenEvent.TweenType.FadeTo]["oncompleteparams"] = typeof(string);
		mappings[iTweenEvent.TweenType.FadeTo]["ignoretimescale"] = typeof(bool);
		
		// FADE UPDATE
		mappings.Add(iTweenEvent.TweenType.FadeUpdate, new Dictionary<string, Type>());
		mappings[iTweenEvent.TweenType.FadeUpdate]["alpha"] = typeof(float);
		mappings[iTweenEvent.TweenType.FadeUpdate]["includechildren"] = typeof(bool);
		mappings[iTweenEvent.TweenType.FadeUpdate]["time"] = typeof(float);
		
		// LOOK FROM
		mappings.Add(iTweenEvent.TweenType.LookFrom, new Dictionary<string, Type>());
		mappings[iTweenEvent.TweenType.LookFrom]["looktarget"] = typeof(Vector3OrTransform);
		mappings[iTweenEvent.TweenType.LookFrom]["axis"] = typeof(string);
		mappings[iTweenEvent.TweenType.LookFrom]["time"] = typeof(float);
		mappings[iTweenEvent.TweenType.LookFrom]["speed"] = typeof(float);
		mappings[iTweenEvent.TweenType.LookFrom]["delay"] = typeof(float);
		mappings[iTweenEvent.TweenType.LookFrom]["easetype"] = typeof(iTween.EaseType);
		mappings[iTweenEvent.TweenType.LookFrom]["looptype"] = typeof(iTween.LoopType);
		mappings[iTweenEvent.TweenType.LookFrom]["onstart"] = typeof(string);
		mappings[iTweenEvent.TweenType.LookFrom]["onstarttarget"] = typeof(GameObject);
		mappings[iTweenEvent.TweenType.LookFrom]["onstartparams"] = typeof(string);
		mappings[iTweenEvent.TweenType.LookFrom]["onupdate"] = typeof(string);
		mappings[iTweenEvent.TweenType.LookFrom]["onupdatetarget"] = typeof(GameObject);
		mappings[iTweenEvent.TweenType.LookFrom]["onupdateparams"] = typeof(string);
		mappings[iTweenEvent.TweenType.LookFrom]["oncomplete"] = typeof(string);
		mappings[iTweenEvent.TweenType.LookFrom]["oncompletetarget"] = typeof(GameObject);
		mappings[iTweenEvent.TweenType.LookFrom]["oncompleteparams"] = typeof(string);
		mappings[iTweenEvent.TweenType.LookFrom]["ignoretimescale"] = typeof(bool);
		
		// LOOK TO
		mappings.Add(iTweenEvent.TweenType.LookTo, new Dictionary<string, Type>());
		mappings[iTweenEvent.TweenType.LookTo]["looktarget"] = typeof(Vector3OrTransform);
		mappings[iTweenEvent.TweenType.LookTo]["axis"] = typeof(string);
		mappings[iTweenEvent.TweenType.LookTo]["time"] = typeof(float);
		mappings[iTweenEvent.TweenType.LookTo]["speed"] = typeof(float);
		mappings[iTweenEvent.TweenType.LookTo]["delay"] = typeof(float);
		mappings[iTweenEvent.TweenType.LookTo]["easetype"] = typeof(iTween.EaseType);
		mappings[iTweenEvent.TweenType.LookTo]["looptype"] = typeof(iTween.LoopType);
		mappings[iTweenEvent.TweenType.LookTo]["onstart"] = typeof(string);
		mappings[iTweenEvent.TweenType.LookTo]["onstarttarget"] = typeof(GameObject);
		mappings[iTweenEvent.TweenType.LookTo]["onstartparams"] = typeof(string);
		mappings[iTweenEvent.TweenType.LookTo]["onupdate"] = typeof(string);
		mappings[iTweenEvent.TweenType.LookTo]["onupdatetarget"] = typeof(GameObject);
		mappings[iTweenEvent.TweenType.LookTo]["onupdateparams"] = typeof(string);
		mappings[iTweenEvent.TweenType.LookTo]["oncomplete"] = typeof(string);
		mappings[iTweenEvent.TweenType.LookTo]["oncompletetarget"] = typeof(GameObject);
		mappings[iTweenEvent.TweenType.LookTo]["oncompleteparams"] = typeof(string);
		mappings[iTweenEvent.TweenType.LookTo]["ignoretimescale"] = typeof(bool);
		
		// LOOK UPDATE
		mappings.Add(iTweenEvent.TweenType.LookUpdate, new Dictionary<string, Type>());
		mappings[iTweenEvent.TweenType.LookUpdate]["looktarget"] = typeof(Vector3OrTransform);
		mappings[iTweenEvent.TweenType.LookUpdate]["axis"] = typeof(string);
		mappings[iTweenEvent.TweenType.LookUpdate]["time"] = typeof(float);
		
		// MOVE ADD
		mappings.Add(iTweenEvent.TweenType.MoveAdd, new Dictionary<string, Type>());
		mappings[iTweenEvent.TweenType.MoveAdd]["amount"] = typeof(Vector3);
		mappings[iTweenEvent.TweenType.MoveAdd]["x"] = typeof(float);
		mappings[iTweenEvent.TweenType.MoveAdd]["y"] = typeof(float);
		mappings[iTweenEvent.TweenType.MoveAdd]["z"] = typeof(float);
		mappings[iTweenEvent.TweenType.MoveAdd]["orienttopath"] = typeof(bool);
		mappings[iTweenEvent.TweenType.MoveAdd]["looktarget"] = typeof(Vector3OrTransform);
		mappings[iTweenEvent.TweenType.MoveAdd]["looktime"] = typeof(float);
		mappings[iTweenEvent.TweenType.MoveAdd]["axis"] = typeof(string);
		mappings[iTweenEvent.TweenType.MoveAdd]["space"] = typeof(Space);
		mappings[iTweenEvent.TweenType.MoveAdd]["time"] = typeof(float);
		mappings[iTweenEvent.TweenType.MoveAdd]["speed"] = typeof(float);
		mappings[iTweenEvent.TweenType.MoveAdd]["delay"] = typeof(float);
		mappings[iTweenEvent.TweenType.MoveAdd]["easetype"] = typeof(iTween.EaseType);
		mappings[iTweenEvent.TweenType.MoveAdd]["looptype"] = typeof(iTween.LoopType);
		mappings[iTweenEvent.TweenType.MoveAdd]["onstart"] = typeof(string);
		mappings[iTweenEvent.TweenType.MoveAdd]["onstarttarget"] = typeof(GameObject);
		mappings[iTweenEvent.TweenType.MoveAdd]["onstartparams"] = typeof(string);
		mappings[iTweenEvent.TweenType.MoveAdd]["onupdate"] = typeof(string);
		mappings[iTweenEvent.TweenType.MoveAdd]["onupdatetarget"] = typeof(GameObject);
		mappings[iTweenEvent.TweenType.MoveAdd]["onupdateparams"] = typeof(string);
		mappings[iTweenEvent.TweenType.MoveAdd]["oncomplete"] = typeof(string);
		mappings[iTweenEvent.TweenType.MoveAdd]["oncompletetarget"] = typeof(GameObject);
		mappings[iTweenEvent.TweenType.MoveAdd]["oncompleteparams"] = typeof(string);
		mappings[iTweenEvent.TweenType.MoveAdd]["ignoretimescale"] = typeof(bool);

		// MOVE BY
		mappings.Add(iTweenEvent.TweenType.MoveBy, new Dictionary<string, Type>());
		mappings[iTweenEvent.TweenType.MoveBy]["time"] = typeof(float);
		mappings[iTweenEvent.TweenType.MoveBy]["speed"] = typeof(float);
		mappings[iTweenEvent.TweenType.MoveBy]["delay"] = typeof(float);
		mappings[iTweenEvent.TweenType.MoveBy]["amount"] = typeof(Vector3);
		mappings[iTweenEvent.TweenType.MoveBy]["x"] = typeof(float);
		mappings[iTweenEvent.TweenType.MoveBy]["y"] = typeof(float);
		mappings[iTweenEvent.TweenType.MoveBy]["z"] = typeof(float);
		mappings[iTweenEvent.TweenType.MoveBy]["orienttopath"] = typeof(bool);
		mappings[iTweenEvent.TweenType.MoveBy]["looktarget"] = typeof(Vector3OrTransform);
		mappings[iTweenEvent.TweenType.MoveBy]["looktime"] = typeof(float);
		mappings[iTweenEvent.TweenType.MoveBy]["axis"] = typeof(string);
		mappings[iTweenEvent.TweenType.MoveBy]["space"] = typeof(Space);
		mappings[iTweenEvent.TweenType.MoveBy]["easetype"] = typeof(iTween.EaseType);
		mappings[iTweenEvent.TweenType.MoveBy]["looptype"] = typeof(iTween.LoopType);
		mappings[iTweenEvent.TweenType.MoveBy]["onstart"] = typeof(string);
		mappings[iTweenEvent.TweenType.MoveBy]["onstarttarget"] = typeof(GameObject);
		mappings[iTweenEvent.TweenType.MoveBy]["onstartparams"] = typeof(string);
		mappings[iTweenEvent.TweenType.MoveBy]["onupdate"] = typeof(string);
		mappings[iTweenEvent.TweenType.MoveBy]["onupdatetarget"] = typeof(GameObject);
		mappings[iTweenEvent.TweenType.MoveBy]["onupdateparams"] = typeof(string);
		mappings[iTweenEvent.TweenType.MoveBy]["oncomplete"] = typeof(string);
		mappings[iTweenEvent.TweenType.MoveBy]["oncompletetarget"] = typeof(GameObject);
		mappings[iTweenEvent.TweenType.MoveBy]["oncompleteparams"] = typeof(string);
		mappings[iTweenEvent.TweenType.MoveBy]["ignoretimescale"] = typeof(bool);
		
		// MOVE FROM
		mappings.Add(iTweenEvent.TweenType.MoveFrom, new Dictionary<string, Type>());
		mappings[iTweenEvent.TweenType.MoveFrom]["position"] = typeof(Vector3OrTransform);
		mappings[iTweenEvent.TweenType.MoveFrom]["path"] = typeof(Vector3OrTransformArray);
		mappings[iTweenEvent.TweenType.MoveFrom]["movetopath"] = typeof(bool);
		mappings[iTweenEvent.TweenType.MoveFrom]["x"] = typeof(float);
		mappings[iTweenEvent.TweenType.MoveFrom]["y"] = typeof(float);
		mappings[iTweenEvent.TweenType.MoveFrom]["z"] = typeof(float);
		mappings[iTweenEvent.TweenType.MoveFrom]["orienttopath"] = typeof(bool);
		mappings[iTweenEvent.TweenType.MoveFrom]["looktarget"] = typeof(Vector3OrTransform);
		mappings[iTweenEvent.TweenType.MoveFrom]["looktime"] = typeof(float);
		mappings[iTweenEvent.TweenType.MoveFrom]["lookahead"] = typeof(float);
		mappings[iTweenEvent.TweenType.MoveFrom]["axis"] = typeof(string);
		mappings[iTweenEvent.TweenType.MoveFrom]["islocal"] = typeof(bool);
		mappings[iTweenEvent.TweenType.MoveFrom]["time"] = typeof(float);
		mappings[iTweenEvent.TweenType.MoveFrom]["speed"] = typeof(float);
		mappings[iTweenEvent.TweenType.MoveFrom]["delay"] = typeof(float);
		mappings[iTweenEvent.TweenType.MoveFrom]["easetype"] = typeof(iTween.EaseType);
		mappings[iTweenEvent.TweenType.MoveFrom]["looptype"] = typeof(iTween.LoopType);
		mappings[iTweenEvent.TweenType.MoveFrom]["onstart"] = typeof(string);
		mappings[iTweenEvent.TweenType.MoveFrom]["onstarttarget"] = typeof(GameObject);
		mappings[iTweenEvent.TweenType.MoveFrom]["onstartparams"] = typeof(string);
		mappings[iTweenEvent.TweenType.MoveFrom]["onupdate"] = typeof(string);
		mappings[iTweenEvent.TweenType.MoveFrom]["onupdatetarget"] = typeof(GameObject);
		mappings[iTweenEvent.TweenType.MoveFrom]["onupdateparams"] = typeof(string);
		mappings[iTweenEvent.TweenType.MoveFrom]["oncomplete"] = typeof(string);
		mappings[iTweenEvent.TweenType.MoveFrom]["oncompletetarget"] = typeof(GameObject);
		mappings[iTweenEvent.TweenType.MoveFrom]["oncompleteparams"] = typeof(string);
		mappings[iTweenEvent.TweenType.MoveFrom]["ignoretimescale"] = typeof(bool);
		
		// MOVE TO
		mappings.Add(iTweenEvent.TweenType.MoveTo, new Dictionary<string, Type>());
		mappings[iTweenEvent.TweenType.MoveTo]["position"] = typeof(Vector3OrTransform);
		mappings[iTweenEvent.TweenType.MoveTo]["path"] = typeof(Vector3OrTransformArray);
		mappings[iTweenEvent.TweenType.MoveTo]["movetopath"] = typeof(bool);
		mappings[iTweenEvent.TweenType.MoveTo]["x"] = typeof(float);
		mappings[iTweenEvent.TweenType.MoveTo]["y"] = typeof(float);
		mappings[iTweenEvent.TweenType.MoveTo]["z"] = typeof(float);
		mappings[iTweenEvent.TweenType.MoveTo]["orienttopath"] = typeof(bool);
		mappings[iTweenEvent.TweenType.MoveTo]["looktarget"] = typeof(Vector3OrTransform);
		mappings[iTweenEvent.TweenType.MoveTo]["looktime"] = typeof(float);
		mappings[iTweenEvent.TweenType.MoveTo]["lookahead"] = typeof(float);
		mappings[iTweenEvent.TweenType.MoveTo]["axis"] = typeof(string);
		mappings[iTweenEvent.TweenType.MoveTo]["islocal"] = typeof(bool);
		mappings[iTweenEvent.TweenType.MoveTo]["time"] = typeof(float);
		mappings[iTweenEvent.TweenType.MoveTo]["speed"] = typeof(float);
		mappings[iTweenEvent.TweenType.MoveTo]["delay"] = typeof(float);
		mappings[iTweenEvent.TweenType.MoveTo]["easetype"] = typeof(iTween.EaseType);
		mappings[iTweenEvent.TweenType.MoveTo]["looptype"] = typeof(iTween.LoopType);
		mappings[iTweenEvent.TweenType.MoveTo]["onstart"] = typeof(string);
		mappings[iTweenEvent.TweenType.MoveTo]["onstarttarget"] = typeof(GameObject);
		mappings[iTweenEvent.TweenType.MoveTo]["onstartparams"] = typeof(string);
		mappings[iTweenEvent.TweenType.MoveTo]["onupdate"] = typeof(string);
		mappings[iTweenEvent.TweenType.MoveTo]["onupdatetarget"] = typeof(GameObject);
		mappings[iTweenEvent.TweenType.MoveTo]["onupdateparams"] = typeof(string);
		mappings[iTweenEvent.TweenType.MoveTo]["oncomplete"] = typeof(string);
		mappings[iTweenEvent.TweenType.MoveTo]["oncompletetarget"] = typeof(GameObject);
		mappings[iTweenEvent.TweenType.MoveTo]["oncompleteparams"] = typeof(string);
		mappings[iTweenEvent.TweenType.MoveTo]["ignoretimescale"] = typeof(bool);
		
		// MOVE UPDATE
		mappings.Add(iTweenEvent.TweenType.MoveUpdate, new Dictionary<string, Type>());
		mappings[iTweenEvent.TweenType.MoveUpdate]["position"] = typeof(Vector3OrTransform);
		mappings[iTweenEvent.TweenType.MoveUpdate]["x"] = typeof(float);
		mappings[iTweenEvent.TweenType.MoveUpdate]["y"] = typeof(float);
		mappings[iTweenEvent.TweenType.MoveUpdate]["z"] = typeof(float);
		mappings[iTweenEvent.TweenType.MoveUpdate]["orienttopath"] = typeof(bool);
		mappings[iTweenEvent.TweenType.MoveUpdate]["looktarget"] = typeof(Vector3OrTransform);
		mappings[iTweenEvent.TweenType.MoveUpdate]["looktime"] = typeof(float);
		mappings[iTweenEvent.TweenType.MoveUpdate]["axis"] = typeof(string);
		mappings[iTweenEvent.TweenType.MoveUpdate]["islocal"] = typeof(bool);
		mappings[iTweenEvent.TweenType.MoveUpdate]["time"] = typeof(float);
		
		// PUNCH POSITION
		mappings.Add(iTweenEvent.TweenType.PunchPosition, new Dictionary<string, Type>());
		mappings[iTweenEvent.TweenType.PunchPosition]["position"] = typeof(Vector3OrTransform);
		mappings[iTweenEvent.TweenType.PunchPosition]["amount"] = typeof(Vector3);
		mappings[iTweenEvent.TweenType.PunchPosition]["x"] = typeof(float);
		mappings[iTweenEvent.TweenType.PunchPosition]["y"] = typeof(float);
		mappings[iTweenEvent.TweenType.PunchPosition]["z"] = typeof(float);
		mappings[iTweenEvent.TweenType.PunchPosition]["space"] = typeof(Space);
		mappings[iTweenEvent.TweenType.PunchPosition]["looktarget"] = typeof(Vector3OrTransform);
		mappings[iTweenEvent.TweenType.PunchPosition]["looktime"] = typeof(float);
		mappings[iTweenEvent.TweenType.PunchPosition]["axis"] = typeof(string);
		mappings[iTweenEvent.TweenType.PunchPosition]["time"] = typeof(float);
		mappings[iTweenEvent.TweenType.PunchPosition]["delay"] = typeof(float);
		mappings[iTweenEvent.TweenType.PunchPosition]["looptype"] = typeof(iTween.LoopType);
		mappings[iTweenEvent.TweenType.PunchPosition]["onstart"] = typeof(string);
		mappings[iTweenEvent.TweenType.PunchPosition]["onstarttarget"] = typeof(GameObject);
		mappings[iTweenEvent.TweenType.PunchPosition]["onstartparams"] = typeof(string);
		mappings[iTweenEvent.TweenType.PunchPosition]["onupdate"] = typeof(string);
		mappings[iTweenEvent.TweenType.PunchPosition]["onupdatetarget"] = typeof(GameObject);
		mappings[iTweenEvent.TweenType.PunchPosition]["onupdateparams"] = typeof(string);
		mappings[iTweenEvent.TweenType.PunchPosition]["oncomplete"] = typeof(string);
		mappings[iTweenEvent.TweenType.PunchPosition]["oncompletetarget"] = typeof(GameObject);
		mappings[iTweenEvent.TweenType.PunchPosition]["oncompleteparams"] = typeof(string);
		mappings[iTweenEvent.TweenType.PunchPosition]["ignoretimescale"] = typeof(bool);

		// PUNCH ROTATION
		mappings.Add(iTweenEvent.TweenType.PunchRotation, new Dictionary<string, Type>());
		mappings[iTweenEvent.TweenType.PunchRotation]["position"] = typeof(Vector3OrTransform);
		mappings[iTweenEvent.TweenType.PunchRotation]["amount"] = typeof(Vector3);
		mappings[iTweenEvent.TweenType.PunchRotation]["x"] = typeof(float);
		mappings[iTweenEvent.TweenType.PunchRotation]["y"] = typeof(float);
		mappings[iTweenEvent.TweenType.PunchRotation]["z"] = typeof(float);
		mappings[iTweenEvent.TweenType.PunchRotation]["space"] = typeof(Space);
		mappings[iTweenEvent.TweenType.PunchRotation]["time"] = typeof(float);
		mappings[iTweenEvent.TweenType.PunchRotation]["delay"] = typeof(float);
		mappings[iTweenEvent.TweenType.PunchRotation]["looptype"] = typeof(iTween.LoopType);
		mappings[iTweenEvent.TweenType.PunchRotation]["onstart"] = typeof(string);
		mappings[iTweenEvent.TweenType.PunchRotation]["onstarttarget"] = typeof(GameObject);
		mappings[iTweenEvent.TweenType.PunchRotation]["onstartparams"] = typeof(string);
		mappings[iTweenEvent.TweenType.PunchRotation]["onupdate"] = typeof(string);
		mappings[iTweenEvent.TweenType.PunchRotation]["onupdatetarget"] = typeof(GameObject);
		mappings[iTweenEvent.TweenType.PunchRotation]["onupdateparams"] = typeof(string);
		mappings[iTweenEvent.TweenType.PunchRotation]["oncomplete"] = typeof(string);
		mappings[iTweenEvent.TweenType.PunchRotation]["oncompletetarget"] = typeof(GameObject);
		mappings[iTweenEvent.TweenType.PunchRotation]["oncompleteparams"] = typeof(string);
		mappings[iTweenEvent.TweenType.PunchRotation]["ignoretimescale"] = typeof(bool);

		// PUNCH SCALE
		mappings.Add(iTweenEvent.TweenType.PunchScale, new Dictionary<string, Type>());
		mappings[iTweenEvent.TweenType.PunchScale]["position"] = typeof(Vector3OrTransform);
		mappings[iTweenEvent.TweenType.PunchScale]["amount"] = typeof(Vector3);
		mappings[iTweenEvent.TweenType.PunchScale]["x"] = typeof(float);
		mappings[iTweenEvent.TweenType.PunchScale]["y"] = typeof(float);
		mappings[iTweenEvent.TweenType.PunchScale]["z"] = typeof(float);
		mappings[iTweenEvent.TweenType.PunchScale]["time"] = typeof(float);
		mappings[iTweenEvent.TweenType.PunchScale]["delay"] = typeof(float);
		mappings[iTweenEvent.TweenType.PunchScale]["looptype"] = typeof(iTween.LoopType);
		mappings[iTweenEvent.TweenType.PunchScale]["onstart"] = typeof(string);
		mappings[iTweenEvent.TweenType.PunchScale]["onstarttarget"] = typeof(GameObject);
		mappings[iTweenEvent.TweenType.PunchScale]["onstartparams"] = typeof(string);
		mappings[iTweenEvent.TweenType.PunchScale]["onupdate"] = typeof(string);
		mappings[iTweenEvent.TweenType.PunchScale]["onupdatetarget"] = typeof(GameObject);
		mappings[iTweenEvent.TweenType.PunchScale]["onupdateparams"] = typeof(string);
		mappings[iTweenEvent.TweenType.PunchScale]["oncomplete"] = typeof(string);
		mappings[iTweenEvent.TweenType.PunchScale]["oncompletetarget"] = typeof(GameObject);
		mappings[iTweenEvent.TweenType.PunchScale]["oncompleteparams"] = typeof(string);
		mappings[iTweenEvent.TweenType.PunchScale]["ignoretimescale"] = typeof(bool);
		
		// ROTATE ADD
		mappings.Add(iTweenEvent.TweenType.RotateAdd, new Dictionary<string, Type>());
		mappings[iTweenEvent.TweenType.RotateAdd]["amount"] = typeof(Vector3);
		mappings[iTweenEvent.TweenType.RotateAdd]["x"] = typeof(float);
		mappings[iTweenEvent.TweenType.RotateAdd]["y"] = typeof(float);
		mappings[iTweenEvent.TweenType.RotateAdd]["z"] = typeof(float);
		mappings[iTweenEvent.TweenType.RotateAdd]["space"] = typeof(Space);
		mappings[iTweenEvent.TweenType.RotateAdd]["time"] = typeof(float);
		mappings[iTweenEvent.TweenType.RotateAdd]["speed"] = typeof(float);
		mappings[iTweenEvent.TweenType.RotateAdd]["delay"] = typeof(float);
		mappings[iTweenEvent.TweenType.RotateAdd]["easetype"] = typeof(iTween.EaseType);
		mappings[iTweenEvent.TweenType.RotateAdd]["looptype"] = typeof(iTween.LoopType);
		mappings[iTweenEvent.TweenType.RotateAdd]["onstart"] = typeof(string);
		mappings[iTweenEvent.TweenType.RotateAdd]["onstarttarget"] = typeof(GameObject);
		mappings[iTweenEvent.TweenType.RotateAdd]["onstartparams"] = typeof(string);
		mappings[iTweenEvent.TweenType.RotateAdd]["onupdate"] = typeof(string);
		mappings[iTweenEvent.TweenType.RotateAdd]["onupdatetarget"] = typeof(GameObject);
		mappings[iTweenEvent.TweenType.RotateAdd]["onupdateparams"] = typeof(string);
		mappings[iTweenEvent.TweenType.RotateAdd]["oncomplete"] = typeof(string);
		mappings[iTweenEvent.TweenType.RotateAdd]["oncompletetarget"] = typeof(GameObject);
		mappings[iTweenEvent.TweenType.RotateAdd]["oncompleteparams"] = typeof(string);
		mappings[iTweenEvent.TweenType.RotateAdd]["ignoretimescale"] = typeof(bool);
		
		// ROTATE BY
		mappings.Add(iTweenEvent.TweenType.RotateBy, new Dictionary<string, Type>());
		mappings[iTweenEvent.TweenType.RotateBy]["amount"] = typeof(Vector3);
		mappings[iTweenEvent.TweenType.RotateBy]["x"] = typeof(float);
		mappings[iTweenEvent.TweenType.RotateBy]["y"] = typeof(float);
		mappings[iTweenEvent.TweenType.RotateBy]["z"] = typeof(float);
		mappings[iTweenEvent.TweenType.RotateBy]["space"] = typeof(Space);
		mappings[iTweenEvent.TweenType.RotateBy]["time"] = typeof(float);
		mappings[iTweenEvent.TweenType.RotateBy]["speed"] = typeof(float);
		mappings[iTweenEvent.TweenType.RotateBy]["delay"] = typeof(float);
		mappings[iTweenEvent.TweenType.RotateBy]["easetype"] = typeof(iTween.EaseType);
		mappings[iTweenEvent.TweenType.RotateBy]["looptype"] = typeof(iTween.LoopType);
		mappings[iTweenEvent.TweenType.RotateBy]["onstart"] = typeof(string);
		mappings[iTweenEvent.TweenType.RotateBy]["onstarttarget"] = typeof(GameObject);
		mappings[iTweenEvent.TweenType.RotateBy]["onstartparams"] = typeof(string);
		mappings[iTweenEvent.TweenType.RotateBy]["onupdate"] = typeof(string);
		mappings[iTweenEvent.TweenType.RotateBy]["onupdatetarget"] = typeof(GameObject);
		mappings[iTweenEvent.TweenType.RotateBy]["onupdateparams"] = typeof(string);
		mappings[iTweenEvent.TweenType.RotateBy]["oncomplete"] = typeof(string);
		mappings[iTweenEvent.TweenType.RotateBy]["oncompletetarget"] = typeof(GameObject);
		mappings[iTweenEvent.TweenType.RotateBy]["oncompleteparams"] = typeof(string);
		mappings[iTweenEvent.TweenType.RotateBy]["ignoretimescale"] = typeof(bool);		
		
		// ROTATE FROM
		mappings.Add(iTweenEvent.TweenType.RotateFrom, new Dictionary<string, Type>());
		mappings[iTweenEvent.TweenType.RotateFrom]["rotation"] = typeof(Vector3OrTransform);
		mappings[iTweenEvent.TweenType.RotateFrom]["x"] = typeof(float);
		mappings[iTweenEvent.TweenType.RotateFrom]["y"] = typeof(float);
		mappings[iTweenEvent.TweenType.RotateFrom]["z"] = typeof(float);
		mappings[iTweenEvent.TweenType.RotateFrom]["islocal"] = typeof(bool);
		mappings[iTweenEvent.TweenType.RotateFrom]["time"] = typeof(float);
		mappings[iTweenEvent.TweenType.RotateFrom]["speed"] = typeof(float);
		mappings[iTweenEvent.TweenType.RotateFrom]["delay"] = typeof(float);
		mappings[iTweenEvent.TweenType.RotateFrom]["easetype"] = typeof(iTween.EaseType);
		mappings[iTweenEvent.TweenType.RotateFrom]["looptype"] = typeof(iTween.LoopType);
		mappings[iTweenEvent.TweenType.RotateFrom]["onstart"] = typeof(string);
		mappings[iTweenEvent.TweenType.RotateFrom]["onstarttarget"] = typeof(GameObject);
		mappings[iTweenEvent.TweenType.RotateFrom]["onstartparams"] = typeof(string);
		mappings[iTweenEvent.TweenType.RotateFrom]["onupdate"] = typeof(string);
		mappings[iTweenEvent.TweenType.RotateFrom]["onupdatetarget"] = typeof(GameObject);
		mappings[iTweenEvent.TweenType.RotateFrom]["onupdateparams"] = typeof(string);
		mappings[iTweenEvent.TweenType.RotateFrom]["oncomplete"] = typeof(string);
		mappings[iTweenEvent.TweenType.RotateFrom]["oncompletetarget"] = typeof(GameObject);
		mappings[iTweenEvent.TweenType.RotateFrom]["oncompleteparams"] = typeof(string);
		mappings[iTweenEvent.TweenType.RotateFrom]["ignoretimescale"] = typeof(bool);		
		
		// ROTATE TO
		mappings.Add(iTweenEvent.TweenType.RotateTo, new Dictionary<string, Type>());
		mappings[iTweenEvent.TweenType.RotateTo]["rotation"] = typeof(Vector3OrTransform);
		mappings[iTweenEvent.TweenType.RotateTo]["x"] = typeof(float);
		mappings[iTweenEvent.TweenType.RotateTo]["y"] = typeof(float);
		mappings[iTweenEvent.TweenType.RotateTo]["z"] = typeof(float);
		mappings[iTweenEvent.TweenType.RotateTo]["islocal"] = typeof(bool);
		mappings[iTweenEvent.TweenType.RotateTo]["time"] = typeof(float);
		mappings[iTweenEvent.TweenType.RotateTo]["speed"] = typeof(float);
		mappings[iTweenEvent.TweenType.RotateTo]["delay"] = typeof(float);
		mappings[iTweenEvent.TweenType.RotateTo]["easetype"] = typeof(iTween.EaseType);
		mappings[iTweenEvent.TweenType.RotateTo]["looptype"] = typeof(iTween.LoopType);
		mappings[iTweenEvent.TweenType.RotateTo]["onstart"] = typeof(string);
		mappings[iTweenEvent.TweenType.RotateTo]["onstarttarget"] = typeof(GameObject);
		mappings[iTweenEvent.TweenType.RotateTo]["onstartparams"] = typeof(string);
		mappings[iTweenEvent.TweenType.RotateTo]["onupdate"] = typeof(string);
		mappings[iTweenEvent.TweenType.RotateTo]["onupdatetarget"] = typeof(GameObject);
		mappings[iTweenEvent.TweenType.RotateTo]["onupdateparams"] = typeof(string);
		mappings[iTweenEvent.TweenType.RotateTo]["oncomplete"] = typeof(string);
		mappings[iTweenEvent.TweenType.RotateTo]["oncompletetarget"] = typeof(GameObject);
		mappings[iTweenEvent.TweenType.RotateTo]["oncompleteparams"] = typeof(string);
		mappings[iTweenEvent.TweenType.RotateTo]["ignoretimescale"] = typeof(bool);			
		
		// ROTATE UPDATE
		mappings.Add(iTweenEvent.TweenType.RotateUpdate, new Dictionary<string, Type>());
		mappings[iTweenEvent.TweenType.RotateUpdate]["rotation"] = typeof(Vector3OrTransform);
		mappings[iTweenEvent.TweenType.RotateUpdate]["x"] = typeof(float);
		mappings[iTweenEvent.TweenType.RotateUpdate]["y"] = typeof(float);
		mappings[iTweenEvent.TweenType.RotateUpdate]["z"] = typeof(float);
		mappings[iTweenEvent.TweenType.RotateUpdate]["islocal"] = typeof(bool);
		mappings[iTweenEvent.TweenType.RotateUpdate]["time"] = typeof(float);
		
		// SCALE ADD
		mappings.Add(iTweenEvent.TweenType.ScaleAdd, new Dictionary<string, Type>());
		mappings[iTweenEvent.TweenType.ScaleAdd]["amount"] = typeof(Vector3);
		mappings[iTweenEvent.TweenType.ScaleAdd]["x"] = typeof(float);
		mappings[iTweenEvent.TweenType.ScaleAdd]["y"] = typeof(float);
		mappings[iTweenEvent.TweenType.ScaleAdd]["z"] = typeof(float);
		mappings[iTweenEvent.TweenType.ScaleAdd]["time"] = typeof(float);
		mappings[iTweenEvent.TweenType.ScaleAdd]["speed"] = typeof(float);
		mappings[iTweenEvent.TweenType.ScaleAdd]["delay"] = typeof(float);
		mappings[iTweenEvent.TweenType.ScaleAdd]["easetype"] = typeof(iTween.EaseType);
		mappings[iTweenEvent.TweenType.ScaleAdd]["looptype"] = typeof(iTween.LoopType);
		mappings[iTweenEvent.TweenType.ScaleAdd]["onstart"] = typeof(string);
		mappings[iTweenEvent.TweenType.ScaleAdd]["onstarttarget"] = typeof(GameObject);
		mappings[iTweenEvent.TweenType.ScaleAdd]["onstartparams"] = typeof(string);
		mappings[iTweenEvent.TweenType.ScaleAdd]["onupdate"] = typeof(string);
		mappings[iTweenEvent.TweenType.ScaleAdd]["onupdatetarget"] = typeof(GameObject);
		mappings[iTweenEvent.TweenType.ScaleAdd]["onupdateparams"] = typeof(string);
		mappings[iTweenEvent.TweenType.ScaleAdd]["oncomplete"] = typeof(string);
		mappings[iTweenEvent.TweenType.ScaleAdd]["oncompletetarget"] = typeof(GameObject);
		mappings[iTweenEvent.TweenType.ScaleAdd]["oncompleteparams"] = typeof(string);
		mappings[iTweenEvent.TweenType.ScaleAdd]["ignoretimescale"] = typeof(bool);
		
		// SCALE BY
		mappings.Add(iTweenEvent.TweenType.ScaleBy, new Dictionary<string, Type>());
		mappings[iTweenEvent.TweenType.ScaleBy]["amount"] = typeof(Vector3);
		mappings[iTweenEvent.TweenType.ScaleBy]["x"] = typeof(float);
		mappings[iTweenEvent.TweenType.ScaleBy]["y"] = typeof(float);
		mappings[iTweenEvent.TweenType.ScaleBy]["z"] = typeof(float);
		mappings[iTweenEvent.TweenType.ScaleBy]["time"] = typeof(float);
		mappings[iTweenEvent.TweenType.ScaleBy]["speed"] = typeof(float);
		mappings[iTweenEvent.TweenType.ScaleBy]["delay"] = typeof(float);
		mappings[iTweenEvent.TweenType.ScaleBy]["easetype"] = typeof(iTween.EaseType);
		mappings[iTweenEvent.TweenType.ScaleBy]["looptype"] = typeof(iTween.LoopType);
		mappings[iTweenEvent.TweenType.ScaleBy]["onstart"] = typeof(string);
		mappings[iTweenEvent.TweenType.ScaleBy]["onstarttarget"] = typeof(GameObject);
		mappings[iTweenEvent.TweenType.ScaleBy]["onstartparams"] = typeof(string);
		mappings[iTweenEvent.TweenType.ScaleBy]["onupdate"] = typeof(string);
		mappings[iTweenEvent.TweenType.ScaleBy]["onupdatetarget"] = typeof(GameObject);
		mappings[iTweenEvent.TweenType.ScaleBy]["onupdateparams"] = typeof(string);
		mappings[iTweenEvent.TweenType.ScaleBy]["oncomplete"] = typeof(string);
		mappings[iTweenEvent.TweenType.ScaleBy]["oncompletetarget"] = typeof(GameObject);
		mappings[iTweenEvent.TweenType.ScaleBy]["oncompleteparams"] = typeof(string);
		mappings[iTweenEvent.TweenType.ScaleBy]["ignoretimescale"] = typeof(bool);
		
		// SCALE FROM
		mappings.Add(iTweenEvent.TweenType.ScaleFrom, new Dictionary<string, Type>());
		mappings[iTweenEvent.TweenType.ScaleFrom]["scale"] = typeof(Vector3OrTransform);
		mappings[iTweenEvent.TweenType.ScaleFrom]["x"] = typeof(float);
		mappings[iTweenEvent.TweenType.ScaleFrom]["y"] = typeof(float);
		mappings[iTweenEvent.TweenType.ScaleFrom]["z"] = typeof(float);
		mappings[iTweenEvent.TweenType.ScaleFrom]["time"] = typeof(float);
		mappings[iTweenEvent.TweenType.ScaleFrom]["speed"] = typeof(float);
		mappings[iTweenEvent.TweenType.ScaleFrom]["delay"] = typeof(float);
		mappings[iTweenEvent.TweenType.ScaleFrom]["easetype"] = typeof(iTween.EaseType);
		mappings[iTweenEvent.TweenType.ScaleFrom]["looptype"] = typeof(iTween.LoopType);
		mappings[iTweenEvent.TweenType.ScaleFrom]["onstart"] = typeof(string);
		mappings[iTweenEvent.TweenType.ScaleFrom]["onstarttarget"] = typeof(GameObject);
		mappings[iTweenEvent.TweenType.ScaleFrom]["onstartparams"] = typeof(string);
		mappings[iTweenEvent.TweenType.ScaleFrom]["onupdate"] = typeof(string);
		mappings[iTweenEvent.TweenType.ScaleFrom]["onupdatetarget"] = typeof(GameObject);
		mappings[iTweenEvent.TweenType.ScaleFrom]["onupdateparams"] = typeof(string);
		mappings[iTweenEvent.TweenType.ScaleFrom]["oncomplete"] = typeof(string);
		mappings[iTweenEvent.TweenType.ScaleFrom]["oncompletetarget"] = typeof(GameObject);
		mappings[iTweenEvent.TweenType.ScaleFrom]["oncompleteparams"] = typeof(string);
		mappings[iTweenEvent.TweenType.ScaleFrom]["ignoretimescale"] = typeof(bool);
		
		// SCALE TO
		mappings.Add(iTweenEvent.TweenType.ScaleTo, new Dictionary<string, Type>());
		mappings[iTweenEvent.TweenType.ScaleTo]["scale"] = typeof(Vector3OrTransform);
		mappings[iTweenEvent.TweenType.ScaleTo]["x"] = typeof(float);
		mappings[iTweenEvent.TweenType.ScaleTo]["y"] = typeof(float);
		mappings[iTweenEvent.TweenType.ScaleTo]["z"] = typeof(float);
		mappings[iTweenEvent.TweenType.ScaleTo]["time"] = typeof(float);
		mappings[iTweenEvent.TweenType.ScaleTo]["speed"] = typeof(float);
		mappings[iTweenEvent.TweenType.ScaleTo]["delay"] = typeof(float);
		mappings[iTweenEvent.TweenType.ScaleTo]["easetype"] = typeof(iTween.EaseType);
		mappings[iTweenEvent.TweenType.ScaleTo]["looptype"] = typeof(iTween.LoopType);
		mappings[iTweenEvent.TweenType.ScaleTo]["onstart"] = typeof(string);
		mappings[iTweenEvent.TweenType.ScaleTo]["onstarttarget"] = typeof(GameObject);
		mappings[iTweenEvent.TweenType.ScaleTo]["onstartparams"] = typeof(string);
		mappings[iTweenEvent.TweenType.ScaleTo]["onupdate"] = typeof(string);
		mappings[iTweenEvent.TweenType.ScaleTo]["onupdatetarget"] = typeof(GameObject);
		mappings[iTweenEvent.TweenType.ScaleTo]["onupdateparams"] = typeof(string);
		mappings[iTweenEvent.TweenType.ScaleTo]["oncomplete"] = typeof(string);
		mappings[iTweenEvent.TweenType.ScaleTo]["oncompletetarget"] = typeof(GameObject);
		mappings[iTweenEvent.TweenType.ScaleTo]["oncompleteparams"] = typeof(string);
		mappings[iTweenEvent.TweenType.ScaleTo]["ignoretimescale"] = typeof(bool);
		
		// SCALE UPDATE
		mappings.Add(iTweenEvent.TweenType.ScaleUpdate, new Dictionary<string, Type>());
		mappings[iTweenEvent.TweenType.ScaleUpdate]["scale"] = typeof(Vector3OrTransform);
		mappings[iTweenEvent.TweenType.ScaleUpdate]["x"] = typeof(float);
		mappings[iTweenEvent.TweenType.ScaleUpdate]["y"] = typeof(float);
		mappings[iTweenEvent.TweenType.ScaleUpdate]["z"] = typeof(float);
		mappings[iTweenEvent.TweenType.ScaleUpdate]["time"] = typeof(float);
		
		// SHAKE POSITION
		mappings.Add(iTweenEvent.TweenType.ShakePosition, new Dictionary<string, Type>());
		mappings[iTweenEvent.TweenType.ShakePosition]["amount"] = typeof(Vector3);
		mappings[iTweenEvent.TweenType.ShakePosition]["x"] = typeof(float);
		mappings[iTweenEvent.TweenType.ShakePosition]["y"] = typeof(float);
		mappings[iTweenEvent.TweenType.ShakePosition]["z"] = typeof(float);
		mappings[iTweenEvent.TweenType.ShakePosition]["space"] = typeof(Space);
		mappings[iTweenEvent.TweenType.ShakePosition]["orienttopath"] = typeof(bool);
		mappings[iTweenEvent.TweenType.ShakePosition]["looktarget"] = typeof(Vector3OrTransform);
		mappings[iTweenEvent.TweenType.ShakePosition]["looktime"] = typeof(float);
		mappings[iTweenEvent.TweenType.ShakePosition]["axis"] = typeof(string);
		mappings[iTweenEvent.TweenType.ShakePosition]["time"] = typeof(float);
		mappings[iTweenEvent.TweenType.ShakePosition]["delay"] = typeof(float);
		mappings[iTweenEvent.TweenType.ShakePosition]["looptype"] = typeof(iTween.LoopType);
		mappings[iTweenEvent.TweenType.ShakePosition]["onstart"] = typeof(string);
		mappings[iTweenEvent.TweenType.ShakePosition]["onstarttarget"] = typeof(GameObject);
		mappings[iTweenEvent.TweenType.ShakePosition]["onstartparams"] = typeof(string);
		mappings[iTweenEvent.TweenType.ShakePosition]["onupdate"] = typeof(string);
		mappings[iTweenEvent.TweenType.ShakePosition]["onupdatetarget"] = typeof(GameObject);
		mappings[iTweenEvent.TweenType.ShakePosition]["onupdateparams"] = typeof(string);
		mappings[iTweenEvent.TweenType.ShakePosition]["oncomplete"] = typeof(string);
		mappings[iTweenEvent.TweenType.ShakePosition]["oncompletetarget"] = typeof(GameObject);
		mappings[iTweenEvent.TweenType.ShakePosition]["oncompleteparams"] = typeof(string);
		mappings[iTweenEvent.TweenType.ShakePosition]["ignoretimescale"] = typeof(bool);
		
		// SHAKE ROTATION
		mappings.Add(iTweenEvent.TweenType.ShakeRotation, new Dictionary<string, Type>());
		mappings[iTweenEvent.TweenType.ShakeRotation]["amount"] = typeof(Vector3);
		mappings[iTweenEvent.TweenType.ShakeRotation]["x"] = typeof(float);
		mappings[iTweenEvent.TweenType.ShakeRotation]["y"] = typeof(float);
		mappings[iTweenEvent.TweenType.ShakeRotation]["z"] = typeof(float);
		mappings[iTweenEvent.TweenType.ShakeRotation]["space"] = typeof(Space);
		mappings[iTweenEvent.TweenType.ShakeRotation]["time"] = typeof(float);
		mappings[iTweenEvent.TweenType.ShakeRotation]["delay"] = typeof(float);
		mappings[iTweenEvent.TweenType.ShakeRotation]["looptype"] = typeof(iTween.LoopType);
		mappings[iTweenEvent.TweenType.ShakeRotation]["onstart"] = typeof(string);
		mappings[iTweenEvent.TweenType.ShakeRotation]["onstarttarget"] = typeof(GameObject);
		mappings[iTweenEvent.TweenType.ShakeRotation]["onstartparams"] = typeof(string);
		mappings[iTweenEvent.TweenType.ShakeRotation]["onupdate"] = typeof(string);
		mappings[iTweenEvent.TweenType.ShakeRotation]["onupdatetarget"] = typeof(GameObject);
		mappings[iTweenEvent.TweenType.ShakeRotation]["onupdateparams"] = typeof(string);
		mappings[iTweenEvent.TweenType.ShakeRotation]["oncomplete"] = typeof(string);
		mappings[iTweenEvent.TweenType.ShakeRotation]["oncompletetarget"] = typeof(GameObject);
		mappings[iTweenEvent.TweenType.ShakeRotation]["oncompleteparams"] = typeof(string);
		mappings[iTweenEvent.TweenType.ShakeRotation]["ignoretimescale"] = typeof(bool);
		
		// SHAKE SCALE
		mappings.Add(iTweenEvent.TweenType.ShakeScale, new Dictionary<string, Type>());
		mappings[iTweenEvent.TweenType.ShakeScale]["amount"] = typeof(Vector3);
		mappings[iTweenEvent.TweenType.ShakeScale]["x"] = typeof(float);
		mappings[iTweenEvent.TweenType.ShakeScale]["y"] = typeof(float);
		mappings[iTweenEvent.TweenType.ShakeScale]["z"] = typeof(float);
		mappings[iTweenEvent.TweenType.ShakeScale]["time"] = typeof(float);
		mappings[iTweenEvent.TweenType.ShakeScale]["delay"] = typeof(float);
		mappings[iTweenEvent.TweenType.ShakeScale]["looptype"] = typeof(iTween.LoopType);
		mappings[iTweenEvent.TweenType.ShakeScale]["onstart"] = typeof(string);
		mappings[iTweenEvent.TweenType.ShakeScale]["onstarttarget"] = typeof(GameObject);
		mappings[iTweenEvent.TweenType.ShakeScale]["onstartparams"] = typeof(string);
		mappings[iTweenEvent.TweenType.ShakeScale]["onupdate"] = typeof(string);
		mappings[iTweenEvent.TweenType.ShakeScale]["onupdatetarget"] = typeof(GameObject);
		mappings[iTweenEvent.TweenType.ShakeScale]["onupdateparams"] = typeof(string);
		mappings[iTweenEvent.TweenType.ShakeScale]["oncomplete"] = typeof(string);
		mappings[iTweenEvent.TweenType.ShakeScale]["oncompletetarget"] = typeof(GameObject);
		mappings[iTweenEvent.TweenType.ShakeScale]["oncompleteparams"] = typeof(string);
		mappings[iTweenEvent.TweenType.ShakeScale]["ignoretimescale"] = typeof(bool);
		
		// STAB
		mappings.Add(iTweenEvent.TweenType.Stab, new Dictionary<string, Type>());
		mappings[iTweenEvent.TweenType.Stab]["audioclip"] = typeof(AudioClip);
		mappings[iTweenEvent.TweenType.Stab]["audiosource"] = typeof(AudioSource);
		mappings[iTweenEvent.TweenType.Stab]["volume"] = typeof(float);
		mappings[iTweenEvent.TweenType.Stab]["pitch"] = typeof(float);
		mappings[iTweenEvent.TweenType.Stab]["delay"] = typeof(float);
		mappings[iTweenEvent.TweenType.Stab]["onstart"] = typeof(string);
		mappings[iTweenEvent.TweenType.Stab]["onstarttarget"] = typeof(GameObject);
		mappings[iTweenEvent.TweenType.Stab]["onstartparams"] = typeof(string);
		mappings[iTweenEvent.TweenType.Stab]["onupdate"] = typeof(string);
		mappings[iTweenEvent.TweenType.Stab]["onupdatetarget"] = typeof(GameObject);
		mappings[iTweenEvent.TweenType.Stab]["onupdateparams"] = typeof(string);
		mappings[iTweenEvent.TweenType.Stab]["oncomplete"] = typeof(string);
		mappings[iTweenEvent.TweenType.Stab]["oncompletetarget"] = typeof(GameObject);
		mappings[iTweenEvent.TweenType.Stab]["oncompleteparams"] = typeof(string);
		
		
	}
}