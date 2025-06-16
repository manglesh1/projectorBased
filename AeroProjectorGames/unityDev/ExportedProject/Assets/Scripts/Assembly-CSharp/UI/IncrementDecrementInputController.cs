using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace UI
{
	public class IncrementDecrementInputController : MonoBehaviour
	{
		private enum IncrementType
		{
			Integer = 0,
			Double = 1
		}

		[SerializeField]
		private Button incrementButton;

		[SerializeField]
		private Button decretmentButton;

		[SerializeField]
		private TMP_InputField field;

		[SerializeField]
		private UnityEvent<double> OnChange;

		[SerializeField]
		private double doubleStepSize;

		[SerializeField]
		private int integerStepSize;

		[SerializeField]
		private IncrementType type;

		private void OnDisable()
		{
			incrementButton.onClick.RemoveListener(IncrementDouble);
			decretmentButton.onClick.RemoveListener(DecrementDouble);
			incrementButton.onClick.RemoveListener(IncrementInteger);
			decretmentButton.onClick.RemoveListener(DecrementInteger);
		}

		private void OnEnable()
		{
			if (type == IncrementType.Double)
			{
				incrementButton.onClick.AddListener(IncrementDouble);
				decretmentButton.onClick.AddListener(DecrementDouble);
			}
			else
			{
				incrementButton.onClick.AddListener(IncrementInteger);
				decretmentButton.onClick.AddListener(DecrementInteger);
			}
		}

		private void DecrementDouble()
		{
			double num = double.Parse(field.text);
			num -= doubleStepSize;
			field.text = num.ToString();
			OnChange?.Invoke(num);
		}

		private void IncrementDouble()
		{
			double num = double.Parse(field.text);
			num += doubleStepSize;
			field.text = num.ToString();
			OnChange?.Invoke(num);
		}

		private void DecrementInteger()
		{
			int num = int.Parse(field.text);
			num -= integerStepSize;
			field.text = num.ToString();
			OnChange?.Invoke(num);
		}

		private void IncrementInteger()
		{
			int num = int.Parse(field.text);
			num += integerStepSize;
			field.text = num.ToString();
			OnChange?.Invoke(num);
		}
	}
}
