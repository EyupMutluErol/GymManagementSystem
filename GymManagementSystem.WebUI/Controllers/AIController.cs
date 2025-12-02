using GymManagementSystem.Business.Abstract;
using GymManagementSystem.WebUI.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GymManagementSystem.WebUI.Controllers;

[Authorize(Roles ="Member")] 
public class AIController : Controller
{
    private readonly IAIService _aiService;

    public AIController(IAIService aiService)
    {
        _aiService = aiService;
    }
     
    [HttpGet]
    public IActionResult Index()
    {
        return View(new AIRequestViewModel());
    }

    [HttpPost]
    public async Task<IActionResult> GeneratePlan(AIRequestViewModel model)
    {
        if (ModelState.IsValid)
        {
            string prompt = $@"
                    Sen dünyanın en iyi spor ve beslenme koçusun. Aşağıdaki özelliklere sahip bir üye için kişiye özel program hazırla.
                    
                    Üye Bilgileri:
                    - Yaş: {model.Age}
                    - Kilo: {model.Weight} kg
                    - Boy: {model.Height} cm
                    - Cinsiyet: {model.Gender}
                    - Hedef: {model.Goal}
                    - Aktivite: {model.ActivityLevel}

                    Lütfen cevabını verirken SADECE HTML kodları kullan ve Bootstrap 5 sınıfları ile görselleştir. Asla ```html veya ``` gibi işaretler koyma.
                    Şu formatı ve yapıyı takip et:

                    1. Başlangıçta <div class='alert alert-info shadow-sm'> içinde üyenin durumunu ve hedefini kısaca özetle ve motive edici bir giriş yap.
                    
                    2. Beslenme Programı için:
                       - Başlığı <h3 class='text-success border-bottom pb-2 mb-3'><i class='fa-solid fa-utensils'></i> Beslenme Programı</h3> olarak yap.
                       - Öğünleri (Kahvaltı, Öğle, Akşam, Ara Öğün) birbirinden ayırmak için <div class='card mb-3 shadow-sm'> yapısını kullan.
                       - Her öğünün başlığını <div class='card-header bg-success text-white fw-bold'> içine yaz.
                       - İçeriği <div class='card-body'> içine liste (<ul>) olarak yaz.

                    3. Antrenman Programı için:
                       - Başlığı <h3 class='text-primary border-bottom pb-2 mb-3 mt-5'><i class='fa-solid fa-dumbbell'></i> Antrenman Programı</h3> olarak yap.
                       - Hareketleri şık bir tablo (table table-hover table-striped) içinde sun.
                       - Tablo başlıkları: Hareket Adı, Set Sayısı, Tekrar Sayısı, Dinlenme Süresi olsun.
                       - Hareketlerin isimlerini <strong> etiketiyle vurgula.

                    4. Sonuç kısmında <div class='alert alert-warning mt-4'> içinde önemli uyarılar (su tüketimi, uyku vb.) ekle.

                    Cevabı direkt <div> ile başlat, <html> veya <body> kullanma.
                ";

            var aiResponse = await _aiService.GetGymWorkoutPlanAsync(prompt);

            model.AIResponse = aiResponse.Replace("```html", "").Replace("```", "");

            return View("Index", model);
        }

        return View("Index", model);
    }

    [HttpGet]
    public async Task<IActionResult> TestModels()
    {
        var result = await _aiService.CheckAvailableModelsAsync();
        return Content(result, "application/json");
    }
}
