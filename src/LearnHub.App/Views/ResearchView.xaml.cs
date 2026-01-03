using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using LearnHub.App.ViewModels;
using LearnHub.Core.Models;

namespace LearnHub.App.Views;

public partial class ResearchView : UserControl
{
    public ResearchView()
    {
        InitializeComponent();
    }

    private void SourceCard_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        if (sender is Border border && border.DataContext is SourceItem source)
        {
            var viewModel = DataContext as ResearchViewModel;
            viewModel?.OpenSourceCommand.Execute(source);
        }
    }

    private void VideoCard_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        if (sender is Border border && border.DataContext is VideoItem video)
        {
            var viewModel = DataContext as ResearchViewModel;
            viewModel?.OpenVideoCommand.Execute(video);
        }
    }
}

