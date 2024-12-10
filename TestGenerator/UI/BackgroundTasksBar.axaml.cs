﻿using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Threading;
using TestGenerator.Core.Services;
using TestGenerator.Shared.Types;

namespace TestGenerator.UI;

public partial class BackgroundTasksBar : UserControl
{
    private IBackgroundTask? _currentTask;
    
    public BackgroundTasksBar()
    {
        InitializeComponent();
        ItemsControl.ItemsSource = AppService.Instance.BackgroundTasks;
        AppService.Instance.BackgroundTasks.CollectionChanged += BackgroundTasksOnCollectionChanged;
    }

    private void BackgroundTasksOnCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        Dispatcher.UIThread.Post(() =>
        {
            if (_currentTask != null)
                _currentTask.ProgressChanged -= CurrentTaskOnProgressChanged;
            _currentTask = AppService.Instance.BackgroundTasks.FirstOrDefault();
            IsVisible = _currentTask != null;
            if (_currentTask != null)
            {
                TaskNameBlock.Text = _currentTask.Name;
                CurrentTaskOnProgressChanged(_currentTask.Progress);
                _currentTask.ProgressChanged += CurrentTaskOnProgressChanged;
            }
            else
            {
                MainButton.Flyout?.Hide();
            }
        });
    }

    private void CurrentTaskOnProgressChanged(double? progress)
    {
        Dispatcher.UIThread.Post(() =>
        {
            ProgressBar.IsIndeterminate = progress == null;
            if (progress != null)
                ProgressBar.Value = progress.Value;
        });
    }
}