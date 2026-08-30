# System Checker

*Note: For the English version, please scroll down below the horizontal line.*

System Checker je "vibecodiran" alat za dijagnostiku sustava izgrađen u WPF-u i modernom .NET-u. Omogućuje korisnicima brzu i jednostavnu provjeru zadovoljava li njihovo računalo specifične hardverske i softverske zahtjeve pomoću preglednog grafičkog sučelja.


## Kako aplikacija radi

Aplikacija se oslanja na **WMI (Windows Management Instrumentation)** kako bi komunicirala s operativnim sustavom i dohvatila stvarne podatke o ugrađenom hardveru. 

### Glavne značajke
* **Korisnički unos zahtjeva:** Na lijevoj strani sučelja korisnik može ručno definirati minimalne zahtjeve za pokretanje nekog softvera (broj CPU jezgri, brzina procesora, RAM, slobodan prostor na C:/ disku, minimalni OS, generacija grafičke kartice te prisutnost obavezne mape).
* **Auto-Detect (Očitaj moj PC):** Pritiskom na gumb, aplikacija automatski skenira računalo i popunjava unosna polja stvarnim specifikacijama vašeg sustava, eliminirajući potrebu za ručnim traženjem podataka.
* **Provjera sustava:** Uspoređuje unesene zahtjeve sa stvarnim stanjem računala. Rezultati se prikazuju na desnoj strani uz jasnu vizualnu indikaciju: zelena boja znači da računalo zadovoljava uvjet, dok crvena označava pad na provjeri.
* **Dinamične Teme:** Aplikacija podržava glatko prebacivanje između tamne (Dark) i svijetle (Light) teme bez ponovnog pokretanja aplikacije.
* **Trajne postavke:** Svi vaši unosi i odabrana tema automatski se spremaju u lokalnu `settings.json` datoteku i ponovno učitavaju pri idućem pokretanju.
* **Pametna detekcija grafičke kartice:** Sustav prepoznaje i klasificira kako diskretne (NVIDIA RTX, AMD RX) tako i integrirane grafičke kartice (AMD Radeon APU, Intel Iris Xe) po godini generacije.


## Tehnologije
* **C# / WPF** (Windows Presentation Foundation)
* **.NET 10**
* `System.Management` (WMI)
* `System.Text.Json` (Serijalizacija postavki)


## Kako pokrenuti i kompajlirati aplikaciju
### Preduvjeti
Potrebno je imati instaliran **[.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)** na vašem računalu.

### Pokretanje tijekom razvoja
Otvorite terminal (ili Command Prompt) u mapi gdje se nalazi vaš projekt i upišite:
```bash
dotnet run
```

### Izrada samostalne .exe datoteke (Publish)
Ako želite podijeliti aplikaciju s korisnicima koji nemaju instaliran .NET, možete kompajlirati projekt u jednu samostalnu izvršnu jedinicu. U terminal upišite:
```bash
dotnet publish -c Release -r win-x64 --self-contained true
```
Nakon završetka, vaša `SystemCheck.exe` datoteka bit će spremna za korištenje i nalazit će se u mapi: `bin\Release\net10.0-windows\win-x64\publish\`

---

# System Checker
*Napomena: verzija na hrvatskom jeziku nalazi se iznad horizontalne crte*

System checker is a "vibecoded" diagnostic tool built in WPF and modern .NET. It allows users to quickly and easily verify if their PC meets specific hardware and software requirements using a clean graphical user interface.


## How the app works

The app relies on **WMI (Windows Management Instrumentation)** to communicate with the operating system and query actual data about the installed hardware.

### Main features
* **User Requirement Input:** On the left side of the interface, the user can manually define the minimum requirements to run a specific piece of software (CPU cores, clock speed, RAM, free space on the C:/ drive, minimum OS, GPU generation, and the presence of a mandatory directory).
* **Auto-Detect Specs:** By pressing a button, the application automatically scans the PC and populates the input fields with your system's actual specifications, eliminating the need to look up data manually.
* **System Check:** Compares the entered requirements against the actual state of the PC. Results are displayed on the right side with a clear visual indication: green means the PC meets the condition, while red indicates a failure.
* **Dynamic Themes:** The application supports seamless switching between Dark and Light themes without needing to restart
* **Persistent Settings:** All your inputs and the selected theme are automatically saved to a local `settings.json` file and reloaded on the next launch.
* **Smart GPU Detection:** The system recognizes and categorizes both discrete (NVIDIA RTX, AMD RX) and modern integrated graphics (AMD Radeon APU, Intel Iris Xe) by their generational release year.


## Technologies
* **C# / WPF** (Windows Presentation Foundation)
* **.NET 10**
* `System.Management` (WMI)
* `System.Text.Json` (Settings serialization)


## How to Build and Run
### Prerequisites
You need to have the **[.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)** installed on your machine

## Running during development
Open a terminal (or Command Prompt) in your project folder and run:
```bash
dotnet run
``` 

### Building a standalone executable (Publish)
If you want to share the app with users who do not have .NET installed, you can compile the project into a single, self-contained executable file. Run this command:
```bash
dotnet publish -c Release -r win-x64 --self-contained true
```
Once completed, your `SystemCheck.exe` will be ready to use and located in: `bin\Release\net10.0-windows\win-x64\publish\`