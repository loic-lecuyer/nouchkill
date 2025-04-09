using Avalonia;
using Avalonia.Controls;
using Avalonia.Threading;
using NouchKill.IO;
using NouchKill.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace NouchKill.Views;

public partial class AgentControl : UserControl
{
    private ObservableCollection<Models.PredictionByClass> _predictions = new ObservableCollection<Models.PredictionByClass>();
    public static readonly DirectProperty<AgentControl, OnnxStream?> OnnxProperty =
AvaloniaProperty.RegisterDirect<AgentControl, OnnxStream?>(
    nameof(Onnx),
    o => o.Onnx,
    (o, v) => o.Onnx = v);

    private OnnxStream? _onnx = null;

    public OnnxStream? Onnx
    {
        get { return _onnx; }
        set
        {
            if (_onnx != null)
            {
                _onnx.OnPredictionsReady -= _onnx_OnPredictionsReady;
            }
            SetAndRaise(OnnxProperty, ref _onnx, value);
            if (_onnx != null)
            {
                _onnx.OnPredictionsReady += _onnx_OnPredictionsReady;
            }
        }
    }



    public AgentControl()
    {
        InitializeComponent();
        PredictionList.ItemsSource = _predictions;
    }

    private async void _onnx_OnPredictionsReady(object? sender, System.Collections.Generic.List<Models.Prediction> e)
    {
        ReadOnlyDictionary<string, List<Models.Prediction>> predictionsByClasses = GetPredictionsByClasses(e);
        foreach (KeyValuePair<string, List<Models.Prediction>> kvp in predictionsByClasses)
        {
            PredictionByClass? existing = (from p in _predictions where p.Label.Equals(kvp.Key) select p).FirstOrDefault();
            if (existing != null)
            {
                existing.AverageConfidence = kvp.Value.Average(p => p.Confidence);
                existing.Count = kvp.Value.Count;
            }
            else
            {
                existing = new PredictionByClass()
                {
                    Label = kvp.Key,
                    AverageConfidence = kvp.Value.Average(p => p.Confidence),
                    Count = kvp.Value.Count
                };
                _predictions.Add(existing);
            }

        }
        List<PredictionByClass> pToRemove = new List<PredictionByClass>();
        foreach (PredictionByClass pdbc in _predictions)
        {
            if (!predictionsByClasses.ContainsKey(pdbc.Label))
            {
                pToRemove.Add(pdbc);
            }
        }

        foreach (PredictionByClass pdbc in pToRemove)
        {
            _predictions.Remove(pdbc);
        }
        try {
            await Dispatcher.UIThread.InvokeAsync(() =>
            {
                if (TxtClasses.Text.Length > 0) {
                    List<string> classesToCheck = new List<string>();
                    if (TxtClasses.Text.IndexOf(",") != -1) {
                        classesToCheck = TxtClasses.Text.Split(',').ToList();
                    } else {
                        classesToCheck.Add(TxtClasses.Text);
                    }
                    bool isInAnyClass = false;
                    foreach (string classToCheck in classesToCheck) {
                        bool isIn = (from p in _predictions where p.Label.Equals(classToCheck) select p).Any();
                        if (isIn) {
                            isInAnyClass = true;
                        }



                    }
                    if (isInAnyClass) {
                        BorderBrush = Avalonia.Media.Brushes.Red;
                        BorderThickness = new Avalonia.Thickness(2);

                    } else {
                        BorderBrush = Avalonia.Media.Brushes.Green;
                        BorderThickness = new Avalonia.Thickness(2);
                    }
                }


            });
        } catch (OperationCanceledException) { }
       

    }

    private static ReadOnlyDictionary<string, List<Prediction>> GetPredictionsByClasses(List<Prediction> e)
    {
        // Group predictions by their labels and return as a ReadOnlyDictionary
        var groupedPredictions = e.GroupBy(p => p.Label)
                                  .ToDictionary(g => g.Key, g => g.ToList());
        return new ReadOnlyDictionary<string, List<Prediction>>(groupedPredictions);
    }
}