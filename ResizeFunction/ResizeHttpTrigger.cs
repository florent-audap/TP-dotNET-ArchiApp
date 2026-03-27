using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;

namespace Company.FunctionResize
{
    public static class ResizeHttpTrigger
    {
        [FunctionName("ResizeHttpTrigger")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            try
            {
                // Validation des paramètres w et h
                if (!int.TryParse(req.Query["w"], out int w) || w <= 0)
                {
                    return new BadRequestObjectResult("Paramètre 'w' invalide. Doit être un entier positif.");
                }
                
                if (!int.TryParse(req.Query["h"], out int h) || h <= 0)
                {
                    return new BadRequestObjectResult("Paramètre 'h' invalide. Doit être un entier positif.");
                }

                // Vérification du body
                if (req.ContentLength == null || req.ContentLength == 0)
                {
                    return new BadRequestObjectResult("Le body de la requête est vide.");
                }

                byte[] targetImageBytes;
                using (var msInput = new MemoryStream())
                {
                    // Récupère le corps du message en mémoire
                    await req.Body.CopyToAsync(msInput);
                    msInput.Position = 0;

                    try
                    {
                        // Charge l'image       
                        using (var image = Image.Load(msInput))
                        {
                            // Effectue la transformation
                            image.Mutate(x => x.Resize(w, h));

                            // Sauvegarde en mémoire               
                            using (var msOutput = new MemoryStream())
                            {
                                image.SaveAsJpeg(msOutput);
                                targetImageBytes = msOutput.ToArray();
                            }
                        }
                    }
                    catch (UnknownImageFormatException)
                    {
                        return new UnsupportedMediaTypeResult();
                    }
                    catch (ImageProcessingException ex)
                    {
                        log.LogError($"Erreur lors du traitement de l'image: {ex.Message}");
                        return new BadRequestObjectResult("Format d'image non supporté ou image corrompue.");
                    }
                }

                return new FileContentResult(targetImageBytes, "image/jpeg");
            }
            catch (Exception ex)
            {
                log.LogError($"Erreur inattendue: {ex.Message}");
                return new ObjectResult("Une erreur inattendue s'est produite.")
                {
                    StatusCode = 500
                };
            }
        }
    }
}
