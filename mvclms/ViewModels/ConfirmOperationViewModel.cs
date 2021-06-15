using Microsoft.AspNetCore.Mvc;

namespace mvclms.ViewModels
{
    public class ConfirmOperationViewModel
    {
        [HiddenInput] public bool Sure { get; set; } = true;
        [HiddenInput] public int Id { get; set; }
    }
}