using UnityEngine;

public class AnimationErrorLogger : MonoBehaviour
{
    //PRUEBA COMmIT
    private Animator m_animator;

    void Start()
    {
        m_animator = GetComponent<Animator>();
        
        if (m_animator != null && m_animator.runtimeAnimatorController != null)
        {
            // Revisa todos los clips de animación que tiene tu personaje
            foreach (AnimationClip clip in m_animator.runtimeAnimatorController.animationClips)
            {
                foreach (AnimationEvent ev in clip.events)
                {
                    // Si encuentra un evento sin función asignada, te avisa en la consola
                    if (string.IsNullOrEmpty(ev.functionName))
                    {
                        Debug.LogError($"⚠️ ¡ERROR ENCONTRADO! El evento vacío está en la animación: LISTA -> CLIPS -> NOMBRE: '{clip.name}'");
                    }
                }
            }
        }
    }
}
