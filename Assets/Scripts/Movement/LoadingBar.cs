using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadingBar : MonoBehaviour
{
    private GameObject BaseCam;
    private Animator ControlPanelAnimator;

    [SerializeField] private bool m_Armed;
    public Transform LookAtTransform;
    public float ActivationDistance = 3;
    public float LookSize;
    

    private bool Loading;
    // Start is called before the first frame update
    void Start()
    {
        if(SceneTransitionManager.IsAvailable())
        {
            BaseCam = SceneTransitionManager.GetMainCamera();
            ControlPanelAnimator = GetComponent<Animator>();
        }
        else
        {
            Destroy(this);
        }
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 cameraLookDirection = BaseCam.transform.forward;
        Vector3 cameraPosition = BaseCam.transform.position;
        
        if(LookAtTransform != null)
        {
            var targetRotation = Quaternion.LookRotation(cameraPosition - LookAtTransform.position);

            LookAtTransform.rotation = Quaternion.Slerp(LookAtTransform.rotation, targetRotation, 1 * Time.deltaTime);
        }


        float distance = Vector3.Distance(LookAtTransform.position, cameraPosition);
        

        if (distance < ActivationDistance && m_Armed)
        {
            float activationAngle = Mathf.Atan(LookSize * 0.5f / distance) * 57.2957f;

            Vector3 directionToLoader = (LookAtTransform.position - BaseCam.transform.position).normalized;
            if (Vector3.Angle(directionToLoader, cameraLookDirection) < activationAngle)
            {
                if (!Loading)
                {
                    Loading = true;
                    StartLoading();
                }
            }
            else if(Loading)
            {
                StopLoading();
                Loading = false;
            }
        } else if(Loading)
        {
            StopLoading();
            Loading = false;
        }
    }

    public void StartLoading()
    {
        if (ControlPanelAnimator != null)
        {
            ControlPanelAnimator.SetBool("Loading", true);
        }
        
        SceneTransitionManager.StartTransition();
    }

    public void StopLoading()
    {
        if (ControlPanelAnimator != null)
        {
            ControlPanelAnimator.SetBool("Loading", false);
        }
        SceneTransitionManager.StopTransition();
    }

    public void TurnOn()
    {
        if (ControlPanelAnimator != null)
        {
            ControlPanelAnimator.SetBool("On", true);
        }
    }

    public void TurnOff()
    {
        if (ControlPanelAnimator != null)
        {
            ControlPanelAnimator.SetBool("On", false);
        }
    }
}
