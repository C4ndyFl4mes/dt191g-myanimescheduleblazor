# My Anime Schedule - Frontend

## Kom igång

#### Instruktioner
1. `git clone https://github.com/C4ndyFl4mes/dt191g-myanimescheduleblazor.git myanimescheduleblazor`
2. Tyvärr måste du byta ut {navigation.BaseUri} till exempelvis http://localhost:5083 i följande filer (det ligger högst up): AuthenticationService, PostService, ScheduleService och UserService.
3. `dotnet run`
4. Klart.

## Beskrivning
En Blazor WebAssembly SPA i C#.NET. Den är uppbyggd av flertalet services, komponenter, DTOs, enums, validators och sidkomponenter. Varje komponent i stort sätt består av två filer i en gemensam mapp, .razor och .cs. Den sistnämnda innehåller all logik och ärver från ComponentBase. Vissa komponenters logikklasser implementerar även IDisposable för att kunna avprenumerera på UserStateService action event OnChange när komponenten inte längre används.

Applikationen består av många olika formulär för att hantera inlägg, scheman och användare. Det finns komponenter för att visa data som användardata eller animedata från Jikan.

Valbara profilbilder och projektikoner finns i wwwroot mappen.