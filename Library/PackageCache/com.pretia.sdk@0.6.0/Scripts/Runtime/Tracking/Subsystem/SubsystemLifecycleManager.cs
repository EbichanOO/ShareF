using UnityEngine;

namespace PretiaArCloud
{
    public class SubsystemLifecycleManager<TSubsystem> : MonoBehaviour
        where TSubsystem : class, ISubsystem
    {
        protected TSubsystem _subsystem;
        private bool _isCouroutineRegistered = false;

        protected virtual void Awake()
        {
            _subsystem = LoaderUtility.GetSubsystem<TSubsystem>();

            if (_subsystem == null)
            {
                throw new System.NullReferenceException($"Unable to get a subsystem with type {typeof(TSubsystem)}");
            }
        }

        protected virtual void OnEnable()
        {
            _subsystem.Resume();
        }

        protected virtual void OnDisable()
        {
            _subsystem.Pause();
        }
    }
}