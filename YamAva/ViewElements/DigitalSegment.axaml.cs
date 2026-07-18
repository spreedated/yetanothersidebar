using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Media;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace YamAva.ViewElements;

public partial class DigitalSegment : UserControl
{
    private static readonly SolidColorBrush _off = SolidColorBrush.Parse("#0e240e");
    private static readonly SolidColorBrush _on = SolidColorBrush.Parse("#66FF66");

    // Map of the segments, 0 = top, 1 = top left, 2 = top right, 3 = middle, 4 = bottom left, 5 = bottom right, 6 = bottom
    private readonly bool[] _map = new bool[7];

    private readonly bool[][] _digits = [
            [true, true, true, false, true, true, true],
            [false, false, true, false, false, true, false],
            [true, false, true, true, true, false, true],
            [true, false, true, true, false, true, true],
            [false, true, true, true, false, true, false],
            [true, true, false, true, false, true, true],
            [true, true, false, true, true, true, true],
            [true, false, true, false, false, true, false],
            [true, true, true, true, true, true, true],
            [true, true, true, true, false, true, true],
            [false, false, false, false, false, false, false]
        ];

    public int CurrentDigit { get; private set; } = 10;

    public event EventHandler<int> DigitChanged;

    public static readonly DirectProperty<DigitalSegment, SolidColorBrush> TopSegmentProperty =
        AvaloniaProperty.RegisterDirect<DigitalSegment, SolidColorBrush>(
            nameof(TopSegment),
            o => o.TopSegment,
            (o, v) => o.TopSegment = v,
            defaultBindingMode: BindingMode.TwoWay);

    private SolidColorBrush _topSegment = _off;
    public SolidColorBrush TopSegment
    {
        get => _topSegment;
        set => this.SetAndRaise(TopSegmentProperty, ref _topSegment, value);
    }

    public static readonly DirectProperty<DigitalSegment, SolidColorBrush> MiddleSegmentProperty =
        AvaloniaProperty.RegisterDirect<DigitalSegment, SolidColorBrush>(
            nameof(MiddleSegment),
            o => o.MiddleSegment,
            (o, v) => o.MiddleSegment = v,
            defaultBindingMode: BindingMode.TwoWay);

    private SolidColorBrush _middleSegment = _off;
    public SolidColorBrush MiddleSegment
    {
        get => _middleSegment;
        set => this.SetAndRaise(MiddleSegmentProperty, ref _middleSegment, value);
    }

    public static readonly DirectProperty<DigitalSegment, SolidColorBrush> BottomSegmentProperty =
        AvaloniaProperty.RegisterDirect<DigitalSegment, SolidColorBrush>(
            nameof(BottomSegment),
            o => o.BottomSegment,
            (o, v) => o.BottomSegment = v,
            defaultBindingMode: BindingMode.TwoWay);

    private SolidColorBrush _bottomSegment = _off;
    public SolidColorBrush BottomSegment
    {
        get => _bottomSegment;
        set => this.SetAndRaise(BottomSegmentProperty, ref _bottomSegment, value);
    }

    public static readonly DirectProperty<DigitalSegment, SolidColorBrush> TopLeftSegmentProperty =
        AvaloniaProperty.RegisterDirect<DigitalSegment, SolidColorBrush>(
            nameof(TopLeftSegment),
            o => o.TopLeftSegment,
            (o, v) => o.TopLeftSegment = v,
            defaultBindingMode: BindingMode.TwoWay);

    private SolidColorBrush _topLeftSegment = _off;
    public SolidColorBrush TopLeftSegment
    {
        get => _topLeftSegment;
        set => this.SetAndRaise(TopLeftSegmentProperty, ref _topLeftSegment, value);
    }

    public static readonly DirectProperty<DigitalSegment, SolidColorBrush> TopRightSegmentProperty =
        AvaloniaProperty.RegisterDirect<DigitalSegment, SolidColorBrush>(
            nameof(TopRightSegment),
            o => o.TopRightSegment,
            (o, v) => o.TopRightSegment = v,
            defaultBindingMode: BindingMode.TwoWay);

    private SolidColorBrush _topRightSegment = _off;
    public SolidColorBrush TopRightSegment
    {
        get => _topRightSegment;
        set => this.SetAndRaise(TopRightSegmentProperty, ref _topRightSegment, value);
    }

    public static readonly DirectProperty<DigitalSegment, SolidColorBrush> BottomLeftSegmentProperty =
        AvaloniaProperty.RegisterDirect<DigitalSegment, SolidColorBrush>(
            nameof(BottomLeftSegment),
            o => o.BottomLeftSegment,
            (o, v) => o.BottomLeftSegment = v,
            defaultBindingMode: BindingMode.TwoWay);

    private SolidColorBrush _bottomLeftSegment = _off;
    public SolidColorBrush BottomLeftSegment
    {
        get => _bottomLeftSegment;
        set => this.SetAndRaise(BottomLeftSegmentProperty, ref _bottomLeftSegment, value);
    }

    public static readonly DirectProperty<DigitalSegment, SolidColorBrush> BottomRightSegmentProperty =
        AvaloniaProperty.RegisterDirect<DigitalSegment, SolidColorBrush>(
            nameof(BottomRightSegment),
            o => o.BottomRightSegment,
            (o, v) => o.BottomRightSegment = v,
            defaultBindingMode: BindingMode.TwoWay);

    private SolidColorBrush _bottomRightSegment = _off;
    public SolidColorBrush BottomRightSegment
    {
        get => _bottomRightSegment;
        set => this.SetAndRaise(BottomRightSegmentProperty, ref _bottomRightSegment, value);
    }

    public DigitalSegment()
    {
        this.InitializeComponent();

        this.DigitChanged += (s, e) => { Debug.Print(e.ToString()); };
    }

    [RelayCommand]
    private void SegmentClick(object obj)
    {
        if (obj.GetType() != typeof(Button))
        {
            return;
        }

        string[] tagParts = ((Button)obj).Tag?.ToString().Split(',');

        PropertyInfo property = typeof(DigitalSegment).GetProperties().FirstOrDefault(x => x.Name == tagParts[0]);
        property.SetValue(this, property.GetValue(this) == _off ? _on : _off);

        _map[int.Parse(tagParts[1])] ^= true;

        bool[] possibleDigit = _digits.FirstOrDefault(x => x.SequenceEqual(_map));

        if (possibleDigit != null)
        {
            this.DigitChanged?.Invoke(this, _digits.IndexOf(possibleDigit));
            this.CurrentDigit = _digits.IndexOf(possibleDigit);
            return;
        }

        this.DigitChanged?.Invoke(this, -1);
        this.CurrentDigit = -1;
    }

    public void SetDigit(int digit)
    {
        if (digit <= -1 || digit >= 11)
        {
            return;
        }

        for (int i = 0; i < _map.Length; i++)
        {
            _map[i] = _digits[digit][i];
        }

        this.TopSegment = _map[0] ? _on : _off;
        this.TopLeftSegment = _map[1] ? _on : _off;
        this.TopRightSegment = _map[2] ? _on : _off;
        this.MiddleSegment = _map[3] ? _on : _off;
        this.BottomLeftSegment = _map[4] ? _on : _off;
        this.BottomRightSegment = _map[5] ? _on : _off;
        this.BottomSegment = _map[6] ? _on : _off;
    }
}