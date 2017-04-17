using UnityEngine;
using System.Collections;
using System;

public enum CameraType
{
    First,
    Second
}

public class CameraController : MonoBehaviour
{

    #region Fields

    [SerializeField]
    private UnityEngine.Camera[] cameras;
    private CameraType activeCamera;

    #endregion

    #region Initialization

    #endregion

    #region Public Members

    public Action<CameraType> ActiveCamera_OnChanged;

    public CameraType Current
    {
        get
        {
            return activeCamera;
        }
        set
        {
            if (activeCamera != value)
            {
                activeCamera = value;

                if (ActiveCamera_OnChanged != null)
                    ActiveCamera_OnChanged(activeCamera);

                if (activeCamera == CameraType.First)
                {
                    cameras[0].gameObject.SetActive(true);
                    cameras[1].gameObject.SetActive(false);
                }
                else
                {
                    cameras[1].gameObject.SetActive(true);
                    cameras[0].gameObject.SetActive(false);
                }
            }
        }
    }

    public void Switch()
    {
        if (activeCamera == CameraType.First)
            Current = CameraType.Second;
        else
            Current = CameraType.First;
    }

    #endregion
}
