# LibraryReservationsAPI

Kitabxanada kitab rezervasiyalarını idarə etmək üçün C# və ASP.NET Core (.NET 9) ilə hazırlanmış laboratoriya layihəsi. Rezervasiyalar serverin yaddaşında `List<Reservation>` daxilində saxlanılır. Proqram yenidən başladıqda siyahı boş olur. API autentifikasiya tələb etmir.

## İşə salmaq

1. .NET 9 SDK və ASP.NET iş yükü quraşdırılmış Visual Studio 2022 (17.12 və ya daha yeni) ilə `LibraryReservationsAPI.sln` faylını açın.
2. `https` profilini seçib **Ctrl + F5** basın. İlk açılışda NuGet paketləri bərpa edilir.
3. Swagger səhifəsi açılır. Ünvanı əl ilə də aça bilərsiniz: `https://localhost:7142/swagger`. OpenAPI sənədi `/swagger/v1/swagger.json` ünvanındadır.

Windows və ya macOS terminalında həll faylının yerləşdiyi qovluqdan da başlatmaq olar:

```sh
dotnet dev-certs https --trust
dotnet run --project LibraryReservationsAPI --launch-profile https
```

## Əməliyyatlar

| Metod | Ünvan | Uğurlu nəticə |
|---|---|---|
| GET | /api/reservations | 200, rezervasiyaların siyahısı |
| GET | /api/reservations/{id} | 200, seçilmiş rezervasiya |
| POST | /api/reservations | 201, yaradılan rezervasiya və Location başlığı |
| PUT | /api/reservations/{id} | 204, bütün məlumatlar yenilənir |
| DELETE | /api/reservations/{id} | 204, rezervasiya silinir |
| GET | /api/reservations/export | 200, bütün rezervasiyaları əhatə edən reservations.json faylı |

POST və PUT sorğularında eyni JSON sahələri göndərilir. ID-ni server təyin edir. Nümunədə götürmə tarixini test etdiyiniz günə və ya daha sonrakı tarixə uyğun seçin.

```json
{
  "bookTitle": "Dədə Qorqud",
  "readerName": "Aysel Əliyeva",
  "libraryBranch": "Mərkəzi",
  "pickupDate": "2027-05-20",
  "copyCount": 2
}
```

Kitab adı, oxucu adı və filial məcburidir. Nüsxə sayı ən azı 1 olmalıdır. Götürmə tarixi serverin yerli təqviminə görə yoxlanılır. Yanlış məlumat üçün 400, mövcud olmayan ID üçün 404 qaytarılır.

## Axtarış və filtrlər

Aşağıdakı parametrləri GET /api/reservations sorğusunda birlikdə də istifadə edə bilərsiniz.

| Parametr | Nümunə | Nəticə |
|---|---|---|
| title | title=Qorqud | Kitab adında göstərilən mətn olan rezervasiyalar |
| branch | branch=Mərkəzi | Seçilmiş filialın rezervasiyaları |
| sort | sort=asc və ya sort=desc | Götürmə tarixinə görə artan və ya azalan sıra |
| futureOnly | futureOnly=true | Götürmə tarixi sabah və ya daha sonra olan rezervasiyalar |
| multipleCopies | multipleCopies=true | Nüsxə sayı 1-dən çox olan rezervasiyalar |

## Postman ilə yoxlama

API işləyərkən Postman-da **Import** seçin və `postman/LibraryReservationsAPI.postman_collection.json` faylını açın. Kolleksiyanı **Run collection** ilə verilmiş ardıcıllıqla icra edin. İlk sorğu gələcək tarix seçir və serverin qaytardığı ID-ni saxlayır. Sonrakı sorğular həmin rezervasiyanı oxuyur, dəyişdirir və silir. Portu dəyişsəniz, kolleksiyada `baseUrl` dəyişənini də yeniləyin.
