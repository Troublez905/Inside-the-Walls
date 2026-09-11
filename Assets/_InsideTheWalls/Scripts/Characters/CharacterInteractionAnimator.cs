using UnityEngine;

namespace InsideTheWalls.Characters
{
    public sealed class CharacterInteractionAnimator : MonoBehaviour
    {
        private Animator animator;
        private static readonly int Guard = Animator.StringToHash("Guard");
        private static readonly int Talk = Animator.StringToHash("Talk");
        private static readonly int Reaction = Animator.StringToHash("Reaction");
        public Animator Animator => animator;

        public static CharacterInteractionAnimator Attach(Transform owner)
        {
            Animator target = owner.GetComponentInChildren<Animator>();
            if (target == null || !target.enabled || target.avatar == null || !target.avatar.isValid
                || !target.isHuman || target.runtimeAnimatorController == null) return null;
            bool guard = false, talk = false, reaction = false;
            foreach (AnimatorControllerParameter parameter in target.parameters)
            {
                guard |= parameter.nameHash == Guard && parameter.type == AnimatorControllerParameterType.Bool;
                talk |= parameter.nameHash == Talk && parameter.type == AnimatorControllerParameterType.Trigger;
                reaction |= parameter.nameHash == Reaction && parameter.type == AnimatorControllerParameterType.Trigger;
            }
            if (!guard || !talk || !reaction) return null;
            var driver = owner.GetComponent<CharacterInteractionAnimator>();
            if (driver == null) driver = owner.gameObject.AddComponent<CharacterInteractionAnimator>();
            driver.animator = target;
            return driver;
        }

        public void SetGuard(bool value)
        {
            if (animator != null) animator.SetBool(Guard, value);
        }

        public void PlayTalk()
        {
            if (animator != null) animator.SetTrigger(Talk);
        }

        public void PlayReaction()
        {
            if (animator != null) animator.SetTrigger(Reaction);
        }

        private void OnDisable()
        {
            if (animator == null) return;
            animator.SetBool(Guard, false);
            animator.ResetTrigger(Talk);
            animator.ResetTrigger(Reaction);
        }
    }
}
