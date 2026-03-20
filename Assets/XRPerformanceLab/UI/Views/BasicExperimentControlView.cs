using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace XRPerformanceLab.UI.Views
{
    public sealed class BasicExperimentControlView : MonoBehaviour
    {
        [Header("Buttons")]
        [SerializeField] private Button renderScaleOnButton;
        [SerializeField] private Button renderScaleOffButton;
        [SerializeField] private Button shadowsOffButton;
        [SerializeField] private Button shadowsRestoreButton;

        [Header("Status")]
        [SerializeField] private TMP_Text statusText;

        public Button RenderScaleOnButton => renderScaleOnButton;
        public Button RenderScaleOffButton => renderScaleOffButton;
        public Button ShadowsOffButton => shadowsOffButton;
        public Button ShadowsRestoreButton => shadowsRestoreButton;

        public void SetStatus(string message)
        {
            if (statusText != null)
                statusText.text = message;
        }
    }
}