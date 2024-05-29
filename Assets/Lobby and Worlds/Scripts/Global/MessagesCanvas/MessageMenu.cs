using FirstGearGames.LobbyAndWorld.Extensions;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace FirstGearGames.LobbyAndWorld.Global.Canvases
{


    public class MessageMenu : MonoBehaviour
    {
        #region Serialized.
        /// <summary>
        /// Image which resides behind text.
        /// </summary>
        [Tooltip("Image which resides behind text.")]
        [SerializeField]
        private Image _backgroundImage;
        /// <summary>
        /// Text to modify.
        /// </summary>
        [Tooltip("Text to modify.")]
        [SerializeField]
        private TextMeshProUGUI _text;
        #endregion


        #region Private.
        /// <summary>
        /// Coroutine of current displayed text.
        /// </summary>
        private Coroutine _runningMessage = null;
        /// <summary>
        /// Text value in text field before changes.
        /// </summary>
        private string _originalText;
        /// <summary>
        /// Color vlaue in the text field before changes.
        /// </summary>
        private Color _originalColor;
        /// <summary>
        /// CanvasGroup on this object.
        /// </summary>
        private CanvasGroup _canvasGroup;
        /// <summary>
        /// True if message is currently persistent.
        /// </summary>
        private bool _persistent = false;
        /// <summary>
        /// Background opacity on start.
        /// </summary>
        private float _defaultBackgroundOpacity;
        #endregion

        private void Awake()
        {
            FirstInitialize();
        }

        /// <summary>
        /// Initializes this script for use.
        /// </summary>
        private void FirstInitialize()
        {
            _canvasGroup = GetComponent<CanvasGroup>();
            _defaultBackgroundOpacity = _backgroundImage.color.a;
            _originalText = _text.text;
            _originalColor = _text.color;
        }

        /// <summary>
        /// Sets to original text values and updates text.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="color"></param>
        public void SetOriginalText(string text, Color color)
        {
            _originalText = text;
            _originalColor = color;
            if (_runningMessage == null)
                SetToOriginals();
        }

        /// <summary>
        /// Shows a message for a duration.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="color"></param>
        /// <param name="duration"></param>
        public void ShowTimedMessage(string text, Color color, float duration = 3f, float? opacity = null)
        {
            if (_runningMessage != null)
                StopCoroutine(_runningMessage);

            _runningMessage = StartCoroutine(__ShowMessage(text, color, duration, opacity));
        }

        /// <summary>
        /// Sets to original text and color.
        /// </summary>
        private void SetToOriginals()
        {
            _text.text = _originalText;
            _text.color = _originalColor;
        }

        /// <summary>
        /// Shows a message for a set duration.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="color"></param>
        /// <param name="duration"></param>
        /// <returns></returns>
        private IEnumerator __ShowMessage(string text, Color color, float duration, float? opacity)
        {
            _canvasGroup.SetActive(true, true);

            //Adjust opacity if not null.
            Color backgroundColor = _backgroundImage.color;
            if (opacity != null)
                backgroundColor.a = opacity.Value;
            else
                backgroundColor.a = _defaultBackgroundOpacity;
            _backgroundImage.color = backgroundColor;

            //Show text for specified time.
            _text.text = text;
            _text.color = color;
            yield return new WaitForSeconds(duration);

            //Set back to original text.
            SetToOriginals();
            _runningMessage = null;

            if (!_persistent)
                _canvasGroup.SetActive(false, true);
        }

        /// <summary>
        /// Shows text persistently until EndPersistentText is called.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="color"></param>
        public void StartPersistentText(string text, Color color)
        {
            _text.text = text;
            _text.color = color;

            _canvasGroup.SetActive(true, true);
        }

        /// <summary>
        /// Shows text persistently using original text values.
        /// </summary>
        public void StartPersistentText()
        {
            SetToOriginals();
            _canvasGroup.SetActive(true, true);
        }

        /// <summary>
        /// Ends persistent text.
        /// </summary>
        public void EndPersistentText()
        {
            _canvasGroup.SetActive(false, true);
        }
    }


}