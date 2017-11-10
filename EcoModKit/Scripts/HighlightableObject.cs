using UnityEngine;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using System;

public class HighlightableObject : MonoBehaviour
{
    public bool seeThrough;
    private HighlightingSystem.Highlighter[] highlighters;
    public Dictionary<Renderer, Material[]> sharedMaterials;

    protected bool unhighlight = false;
    public bool makeMaterialCopy = false;

    public Action OnHighlight;
    public Action OnUnHighlight;

    public void Highlight(Color highlightColor)
    {
        if (OnHighlight != null)
            OnHighlight();

        if (this.highlighters == null)
        {
            var renderers = this.gameObject.GetComponentsInChildren<Renderer>(true);

            highlighters = new HighlightingSystem.Highlighter[renderers.Count()];
            if (makeMaterialCopy)
            {
                sharedMaterials = new Dictionary<Renderer, Material[]>();
                foreach (var renderer in renderers)
                {
                    // use an instance of the material, to fix batching problems with highlighting system
                    // revert back to shared when unhighlighted
                    sharedMaterials[renderer] = renderer.sharedMaterials;
                    Material[] materials = renderer.materials;
                    renderer.materials = materials;
                }
            }

            int index = 0;
            foreach (var renderer in renderers)
            {
                var highlighter = renderer.gameObject.GetComponent<HighlightingSystem.Highlighter>();
                if (highlighter == null)
                    highlighter = renderer.gameObject.AddComponent<HighlightingSystem.Highlighter>();

                this.highlighters[index++] = highlighter;
            }
        }

        foreach (var highlighter in this.highlighters)
        {
            if (highlighter == null)
                continue;
            if (highlighter.GetComponentInParent<HighlightableObject>() == this)
            {
                highlighter.ConstantOn(highlightColor);
                highlighter.seeThrough = seeThrough;
            }
            else
            {
                highlighter.ConstantOff();
            }
        }

        this.unhighlight = false;
        this.enabled = true;
    }

    public void UnHighlight()
    {
        if (this.highlighters == null)
            return;

        foreach (var highlighter in this.highlighters)
            if (highlighter != null)
                highlighter.ConstantOff();

        this.unhighlight = true;
        this.enabled = true;
    }

    public void LateUpdate()
    {
        if (this.highlighters != null && this.unhighlight)
        {
            bool doneHighlighting = true;
            foreach (var highlighter in this.highlighters)
            {
                if (highlighter.TransitionActive)
                {
                    doneHighlighting = false;
                    break;
                }
            }

            if (doneHighlighting)
            {
                foreach (var highlighter in this.highlighters)
                    highlighter.Die();

                this.highlighters = null;

                // restore materials
                if (this.makeMaterialCopy && sharedMaterials != null)
                {
                    foreach (var renderer in sharedMaterials.Keys)
                        renderer.sharedMaterials = sharedMaterials[renderer];

                    sharedMaterials.Clear();
                    sharedMaterials = null;
                }

                if (OnUnHighlight != null)
                    OnUnHighlight();
            }
        }
        else
        {
            this.enabled = false;
        }
    }

    public void RefreshHighlight()
    {
        if (this.highlighters != null)
            foreach (var highlighter in highlighters)
                highlighter.ReinitMaterials();
    }
}
