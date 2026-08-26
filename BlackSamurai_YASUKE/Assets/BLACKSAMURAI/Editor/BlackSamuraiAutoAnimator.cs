using UnityEditor;
using UnityEngine;
using UnityEditor.Animations;
using System.Collections.Generic;

public static class BlackSamuraiAutoAnimator
{
    static readonly Dictionary<string,int> Frames = new()
    {
        {"idle",4},{"walk",4},{"run",4},{"jump",4},{"attack1",4},{"attack2",3},
        {"dash",3},{"air_attack",3},{"special_spin",4},{"hurt",4},{"death",3},
        {"crouch",3},{"block",2},{"projectile",3}
    };

    [MenuItem("BLACKSAMURAI/Build Sprite Slices + Animator")]
    public static void Build()
    {
        const string sprites = "Assets/BLACKSAMURAI/Sprites";
        const string anims = "Assets/BLACKSAMURAI/Animations";
        EnsureFolder("Assets/BLACKSAMURAI");
        EnsureFolder(anims);

        foreach (var p in Frames)
            Slice(sprites + "/" + p.Key + ".png", p.Key, p.Value);

        AssetDatabase.Refresh();

        foreach (var p in Frames)
            AssetDatabase.ImportAsset(sprites + "/" + p.Key + ".png",
                ImportAssetOptions.ForceUpdate);

        AssetDatabase.Refresh();
        CreateController(sprites, anims);
        AssetDatabase.SaveAssets();
        Debug.Log("BLACKSAMURAI: Animator and sprite slices created.");
    }

    static void Slice(string path, string name, int count)
    {
        var importer = AssetImporter.GetAtPath(path) as TextureImporter;
        if (importer == null) return;

        importer.textureType = TextureImporterType.Sprite;
        importer.spriteImportMode = SpriteImportMode.Multiple;
        importer.filterMode = FilterMode.Bilinear;
        importer.textureCompression = TextureImporterCompression.Uncompressed;
        importer.alphaIsTransparency = true;
        importer.spritePixelsPerUnit = 100;

        var tex = AssetDatabase.LoadAssetAtPath<Texture2D>(path);
        if (tex == null) return;

        int fw = tex.width / count;
        var data = new List<SpriteMetaData>();

        for (int i = 0; i < count; i++)
        {
            int x = i * fw;
            int w = (i == count - 1) ? tex.width - x : fw;
            data.Add(new SpriteMetaData {
                name = name + "_" + i.ToString("00"),
                rect = new Rect(x, 0, w, tex.height),
                pivot = new Vector2(.5f,.5f),
                alignment = (int)SpriteAlignment.Center
            });
        }

        importer.spritesheet = data.ToArray();
        importer.SaveAndReimport();
    }

    static AnimationClip MakeClip(string stateName, string strip,
        string spritesFolder, float fps, bool loop)
    {
        string path = spritesFolder + "/" + strip + ".png";
        Object[] objs = AssetDatabase.LoadAllAssetsAtPath(path);
        var sprites = new List<Sprite>();

        foreach (var o in objs)
            if (o is Sprite) sprites.Add((Sprite)o);

        sprites.Sort((a,b) => a.name.CompareTo(b.name));

        string clipPath = "Assets/BLACKSAMURAI/Animations/" + stateName + ".anim";
        var clip = AssetDatabase.LoadAssetAtPath<AnimationClip>(clipPath);
        if (clip == null)
        {
            clip = new AnimationClip();
            AssetDatabase.CreateAsset(clip, clipPath);
        }

        var old = AnimationUtility.GetObjectReferenceCurveBindings(clip);
        foreach (var b in old)
            AnimationUtility.SetObjectReferenceCurve(clip,b,null);

        var keys = new ObjectReferenceKeyframe[sprites.Count];
        for (int i=0;i<sprites.Count;i++)
            keys[i] = new ObjectReferenceKeyframe {
                time=i/fps, value=sprites[i]
            };

        var binding = EditorCurveBinding.PPtrCurve(
            "", typeof(SpriteRenderer), "m_Sprite");

        AnimationUtility.SetObjectReferenceCurve(clip,binding,keys);
        var settings = AnimationUtility.GetAnimationClipSettings(clip);
        settings.loopTime = loop;
        AnimationUtility.SetAnimationClipSettings(clip,settings);
        clip.frameRate = fps;
        EditorUtility.SetDirty(clip);
        return clip;
    }

    static void CreateController(string sprites, string anims)
    {
        string controllerPath = anims + "/BlackSamurai.controller";
        var c = AssetDatabase.LoadAssetAtPath<AnimatorController>(controllerPath);
        if (c == null)
            c = AnimatorController.CreateAnimatorControllerAtPath(controllerPath);

        c.parameters = new AnimatorControllerParameter[0];
        Add(c,"Speed",AnimatorControllerParameterType.Float);
        Add(c,"Grounded",AnimatorControllerParameterType.Bool);
        Add(c,"VerticalVelocity",AnimatorControllerParameterType.Float);
        Add(c,"Attack",AnimatorControllerParameterType.Trigger);
        Add(c,"HeavyAttack",AnimatorControllerParameterType.Trigger);
        Add(c,"Dash",AnimatorControllerParameterType.Trigger);
        Add(c,"Hurt",AnimatorControllerParameterType.Trigger);
        Add(c,"Dead",AnimatorControllerParameterType.Bool);
        Add(c,"Crouch",AnimatorControllerParameterType.Bool);
        Add(c,"Block",AnimatorControllerParameterType.Bool);

        var sm = c.layers[0].stateMachine;
        foreach (var s in sm.states) sm.RemoveState(s.state);

        State(sm,"Idle","idle",8,true,sprites);
        State(sm,"Walk","walk",10,true,sprites);
        State(sm,"Run","run",12,true,sprites);
        State(sm,"Jump","jump",10,false,sprites);
        State(sm,"Attack1","attack1",14,false,sprites);
        State(sm,"Attack2","attack2",10,false,sprites);
        State(sm,"Dash","dash",14,false,sprites);
        State(sm,"AirAttack","air_attack",12,false,sprites);
        State(sm,"SpecialSpin","special_spin",12,false,sprites);
        State(sm,"Hurt","hurt",10,false,sprites);
        State(sm,"Death","death",8,false,sprites);
        State(sm,"Crouch","crouch",8,true,sprites);
        State(sm,"Block","block",8,true,sprites);

        sm.defaultState = Find(sm,"Idle");

        Transition(Find(sm,"Idle"),Find(sm,"Walk"),"Speed",
            AnimatorConditionMode.Greater,0.1f);
        Transition(Find(sm,"Walk"),Find(sm,"Idle"),"Speed",
            AnimatorConditionMode.Less,0.1f);
        Transition(Find(sm,"Walk"),Find(sm,"Run"),"Speed",
            AnimatorConditionMode.Greater,0.8f);
        Transition(Find(sm,"Run"),Find(sm,"Walk"),"Speed",
            AnimatorConditionMode.Less,0.8f);

        Any(sm,Find(sm,"Attack1"),"Attack");
        Any(sm,Find(sm,"Attack2"),"HeavyAttack");
        Any(sm,Find(sm,"Dash"),"Dash");
        Any(sm,Find(sm,"Hurt"),"Hurt");
        Any(sm,Find(sm,"Death"),"Dead");
        Any(sm,Find(sm,"Crouch"),"Crouch");
        Any(sm,Find(sm,"Block"),"Block");

        foreach (string n in new[]{"Attack1","Attack2","Dash","AirAttack","SpecialSpin","Hurt"})
        {
            var s=Find(sm,n);
            var t=s.AddTransition(Find(sm,"Idle"));
            t.hasExitTime=true; t.exitTime=.9f; t.duration=.05f;
        }

        EditorUtility.SetDirty(c);
    }

    static AnimatorState State(AnimatorStateMachine sm,string name,string strip,
        float fps,bool loop,string sprites)
    {
        var s=sm.AddState(name);
        s.motion=MakeClip(name,strip,sprites,fps,loop);
        return s;
    }

    static AnimatorState Find(AnimatorStateMachine sm,string name)
    {
        foreach(var s in sm.states)
            if(s.state.name==name) return s.state;
        return null;
    }

    static void Add(AnimatorController c,string n,AnimatorControllerParameterType t)
        => c.AddParameter(n,t);

    static void Transition(AnimatorState a,AnimatorState b,string p,
        AnimatorConditionMode mode,float value)
    {
        var t=a.AddTransition(b);
        t.hasExitTime=false; t.duration=.05f;
        t.AddCondition(mode,value,p);
    }

    static void Any(AnimatorStateMachine sm,AnimatorState s,string p)
    {
        var t=sm.AddAnyStateTransition(s);
        t.hasExitTime=false; t.duration=.05f;
        t.AddCondition(AnimatorConditionMode.If,0,p);
    }

    static void Any(AnimatorStateMachine sm,AnimatorState s,string p,
        AnimatorConditionMode mode,float value)
    {
        var t=sm.AddAnyStateTransition(s);
        t.hasExitTime=false; t.duration=.05f;
        t.AddCondition(mode,value,p);
    }

    static void EnsureFolder(string path)
    {
        var parts=path.Split('/');
        string current=parts[0];
        for(int i=1;i<parts.Length;i++)
        {
            string next=current+"/"+parts[i];
            if(!AssetDatabase.IsValidFolder(next))
                AssetDatabase.CreateFolder(current,parts[i]);
            current=next;
        }
    }
}
