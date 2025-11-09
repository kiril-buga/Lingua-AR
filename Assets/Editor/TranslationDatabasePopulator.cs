using UnityEngine;
using UnityEditor;

/// <summary>
/// Pre-populated translation database.
/// Run this ONCE to populate your TranslationDatabaseSO with all 618 translations.
/// </summary>
public class TranslationDatabasePopulator : EditorWindow
{
    private TranslationDatabaseSO _targetDatabase;
    private bool _isPopulated = false;

    [MenuItem("LinguaAR/Populate Translation Database (Pre-Made)")]
    public static void ShowWindow()
    {
        GetWindow<TranslationDatabasePopulator>("Populate Translations");
    }

    private void OnGUI()
    {
        EditorGUILayout.LabelField("Translation Database Populator", EditorStyles.boldLabel);
        EditorGUILayout.Space();

        EditorGUILayout.HelpBox(
            "This will populate your database with all 618 pre-translated entries.\n" +
            "Languages: French, German, Italian\n\n" +
            "This is instant and requires no API calls!",
            MessageType.Info
        );

        EditorGUILayout.Space();

        _targetDatabase = (TranslationDatabaseSO)EditorGUILayout.ObjectField(
            "Target Database",
            _targetDatabase,
            typeof(TranslationDatabaseSO),
            false
        );

        EditorGUILayout.Space();

        // Show current entry count
        if (_targetDatabase != null)
        {
            int currentCount = GetEntryCount();
            EditorGUILayout.LabelField("Current Entries:", $"{currentCount} / 206");

            if (currentCount > 0 && currentCount != 206)
            {
                EditorGUILayout.HelpBox(
                    $"Warning: Database has {currentCount} entries (expected 206). Click 'Populate Database Now' to fix this.",
                    MessageType.Warning
                );
            }
        }

        EditorGUILayout.Space();

        GUI.enabled = !_isPopulated && _targetDatabase != null;
        if (GUILayout.Button("Populate Database Now", GUILayout.Height(40)))
        {
            PopulateDatabase();
        }
        GUI.enabled = true;

        if (_isPopulated)
        {
            EditorGUILayout.HelpBox("✓ Database populated successfully!", MessageType.Info);
        }
    }

    private void PopulateDatabase()
    {
        if (_targetDatabase == null)
        {
            EditorUtility.DisplayDialog("Error", "Please assign a Translation Database!", "OK");
            return;
        }

        // Clear existing entries to prevent duplicates
        Debug.Log("[TranslationPopulator] Clearing existing entries...");
        ClearDatabase();

        Debug.Log("[TranslationPopulator] Starting database population...");

        // People & Body Parts
        Add("human_face", "Human Face", "Visage humain", "Menschliches Gesicht", "Volto umano");
        Add("human_hand", "Human Hand", "Main humaine", "Menschliche Hand", "Mano umana");
        Add("person", "Person", "Personne", "Person", "Persona");
        Add("skull", "Skull", "Crâne", "Schädel", "Teschio");

        // Vehicles
        Add("aircraft", "Aircraft", "Avion", "Flugzeug", "Aereo");
        Add("bicycle", "Bicycle", "Vélo", "Fahrrad", "Bicicletta");
        Add("boat", "Boat", "Bateau", "Boot", "Barca");
        Add("bus", "Bus", "Bus", "Bus", "Autobus");
        Add("car", "Car", "Voiture", "Auto", "Macchina");
        Add("cart", "Cart", "Chariot", "Wagen", "Carrello");
        Add("motorcycle", "Motorcycle", "Moto", "Motorrad", "Motocicletta");
        Add("taxi", "Taxi", "Taxi", "Taxi", "Taxi");
        Add("train", "Train", "Train", "Zug", "Treno");
        Add("truck", "Truck", "Camion", "Lastwagen", "Camion");
        Add("vehicle", "Vehicle", "Véhicule", "Fahrzeug", "Veicolo");
        Add("wheel", "Wheel", "Roue", "Rad", "Ruota");
        Add("wheelchair", "Wheelchair", "Fauteuil roulant", "Rollstuhl", "Sedia a rotelle");

        // Street & Infrastructure
        Add("bench", "Bench", "Banc", "Bank", "Panchina");
        Add("billboard", "Billboard", "Panneau publicitaire", "Werbetafel", "Cartellone pubblicitario");
        Add("christmas_tree", "Christmas Tree", "Sapin de Noël", "Weihnachtsbaum", "Albero di Natale");
        Add("door", "Door", "Porte", "Tür", "Porta");
        Add("door_handle", "Door Handle", "Poignée de porte", "Türklinke", "Maniglia della porta");
        Add("fire_hydrant", "Fire Hydrant", "Bouche d'incendie", "Hydrant", "Idrante");
        Add("flag", "Flag", "Drapeau", "Flagge", "Bandiera");
        Add("parking_meter", "Parking Meter", "Parcmètre", "Parkuhr", "Parchimetro");
        Add("poster", "Poster", "Affiche", "Plakat", "Poster");
        Add("sculpture", "Sculpture", "Sculpture", "Skulptur", "Scultura");
        Add("street_light", "Street Light", "Lampadaire", "Straßenlaterne", "Lampione");
        Add("traffic_light", "Traffic Light", "Feu de circulation", "Ampel", "Semaforo");
        Add("traffic_sign", "Traffic Sign", "Panneau de signalisation", "Verkehrsschild", "Segnale stradale");
        Add("waste_container", "Waste Container", "Poubelle", "Mülleimer", "Contenitore dei rifiuti");
        Add("water_feature", "Water Feature", "Fontaine", "Wasserspiel", "Fontana");
        Add("window", "Window", "Fenêtre", "Fenster", "Finestra");

        // Clothing & Accessories
        Add("backpack", "Backpack", "Sac à dos", "Rucksack", "Zaino");
        Add("clothing", "Clothing", "Vêtements", "Kleidung", "Abbigliamento");
        Add("coat", "Coat", "Manteau", "Mantel", "Cappotto");
        Add("dress", "Dress", "Robe", "Kleid", "Vestito");
        Add("fedora", "Fedora", "Fedora", "Fedora", "Fedora");
        Add("footwear", "Footwear", "Chaussures", "Schuhe", "Calzature");
        Add("glasses", "Glasses", "Lunettes", "Brille", "Occhiali");
        Add("handbag", "Handbag", "Sac à main", "Handtasche", "Borsa");
        Add("headwear", "Headwear", "Couvre-chef", "Kopfbedeckung", "Copricapo");
        Add("roller_skates", "Roller Skates", "Patins à roulettes", "Rollschuhe", "Pattini a rotelle");
        Add("shirt", "Shirt", "Chemise", "Hemd", "Camicia");
        Add("shorts", "Shorts", "Short", "Shorts", "Pantaloncini");
        Add("skirt", "Skirt", "Jupe", "Rock", "Gonna");
        Add("sock", "Sock", "Chaussette", "Socke", "Calzino");
        Add("suit", "Suit", "Costume", "Anzug", "Abito");
        Add("suitcase", "Suitcase", "Valise", "Koffer", "Valigia");
        Add("tie", "Tie", "Cravate", "Krawatte", "Cravatta");
        Add("trousers", "Trousers", "Pantalon", "Hose", "Pantaloni");
        Add("umbrella", "Umbrella", "Parapluie", "Regenschirm", "Ombrello");

        // Sports & Recreation
        Add("baseball_bat", "Baseball Bat", "Batte de baseball", "Baseballschläger", "Mazza da baseball");
        Add("baseball_glove", "Baseball Glove", "Gant de baseball", "Baseballhandschuh", "Guanto da baseball");
        Add("football", "Football", "Football", "Fußball", "Pallone");
        Add("frisbee", "Frisbee", "Frisbee", "Frisbee", "Frisbee");
        Add("kite", "Kite", "Cerf-volant", "Drachen", "Aquilone");
        Add("paddle", "Paddle", "Pagaie", "Paddel", "Pagaia");
        Add("rugby_ball", "Rugby Ball", "Ballon de rugby", "Rugbyball", "Palla da rugby");
        Add("skateboard", "Skateboard", "Skateboard", "Skateboard", "Skateboard");
        Add("skis", "Skis", "Skis", "Ski", "Sci");
        Add("snowboard", "Snowboard", "Snowboard", "Snowboard", "Snowboard");
        Add("sports_ball", "Sports Ball", "Ballon de sport", "Sportball", "Palla sportiva");
        Add("surfboard", "Surfboard", "Planche de surf", "Surfbrett", "Tavola da surf");
        Add("tennis_ball", "Tennis Ball", "Balle de tennis", "Tennisball", "Palla da tennis");
        Add("tennis_racket", "Tennis Racket", "Raquette de tennis", "Tennisschläger", "Racchetta da tennis");

        // Musical Instruments
        Add("accordion", "Accordion", "Accordéon", "Akkordeon", "Fisarmonica");
        Add("brass_instrument", "Brass Instrument", "Instrument en laiton", "Blechblasinstrument", "Strumento a ottone");
        Add("drum", "Drum", "Tambour", "Trommel", "Tamburo");
        Add("flute", "Flute", "Flûte", "Flöte", "Flauto");
        Add("guitar", "Guitar", "Guitare", "Gitarre", "Chitarra");
        Add("musical_instrument", "Musical Instrument", "Instrument de musique", "Musikinstrument", "Strumento musicale");
        Add("piano", "Piano", "Piano", "Klavier", "Pianoforte");
        Add("string_instrument", "String Instrument", "Instrument à cordes", "Saiteninstrument", "Strumento a corde");
        Add("violin", "Violin", "Violon", "Geige", "Violino");

        // Food & Drink
        Add("apple", "Apple", "Pomme", "Apfel", "Mela");
        Add("banana", "Banana", "Banane", "Banane", "Banana");
        Add("berry", "Berry", "Baie", "Beere", "Bacca");
        Add("broccoli", "Broccoli", "Brocoli", "Brokkoli", "Broccoli");
        Add("carrot", "Carrot", "Carotte", "Karotte", "Carota");
        Add("citrus", "Citrus", "Agrume", "Zitrusfrucht", "Agrume");
        Add("coconut", "Coconut", "Noix de coco", "Kokosnuss", "Cocco");
        Add("egg", "Egg", "Œuf", "Ei", "Uovo");
        Add("food", "Food", "Nourriture", "Essen", "Cibo");
        Add("grape", "Grape", "Raisin", "Traube", "Uva");
        Add("mushroom", "Mushroom", "Champignon", "Pilz", "Fungo");
        Add("pear", "Pear", "Poire", "Birne", "Pera");
        Add("pumpkin", "Pumpkin", "Citrouille", "Kürbis", "Zucca");
        Add("tomato", "Tomato", "Tomate", "Tomate", "Pomodoro");
        Add("drink", "Drink", "Boisson", "Getränk", "Bevanda");
        Add("hot_drink", "Hot Drink", "Boisson chaude", "Heißgetränk", "Bevanda calda");
        Add("juice", "Juice", "Jus", "Saft", "Succo");
        Add("bread", "Bread", "Pain", "Brot", "Pane");
        Add("cake", "Cake", "Gâteau", "Kuchen", "Torta");
        Add("cheese", "Cheese", "Fromage", "Käse", "Formaggio");
        Add("dessert", "Dessert", "Dessert", "Nachtisch", "Dolce");
        Add("donut", "Donut", "Beignet", "Donut", "Ciambella");
        Add("fast_food", "Fast Food", "Restauration rapide", "Fast Food", "Fast food");
        Add("french_fries", "French Fries", "Frites", "Pommes frites", "Patatine fritte");
        Add("hamburger", "Hamburger", "Hamburger", "Hamburger", "Hamburger");
        Add("hot_dog", "Hot Dog", "Hot-dog", "Hot Dog", "Hot dog");
        Add("ice_cream", "Ice Cream", "Glace", "Eiscreme", "Gelato");
        Add("pizza", "Pizza", "Pizza", "Pizza", "Pizza");
        Add("sandwich", "Sandwich", "Sandwich", "Sandwich", "Panino");
        Add("sushi", "Sushi", "Sushi", "Sushi", "Sushi");

        // Household Items
        Add("bed", "Bed", "Lit", "Bett", "Letto");
        Add("chair", "Chair", "Chaise", "Stuhl", "Sedia");
        Add("couch", "Couch", "Canapé", "Sofa", "Divano");
        Add("furniture", "Furniture", "Meuble", "Möbel", "Mobile");
        Add("shelves", "Shelves", "Étagères", "Regale", "Scaffali");
        Add("storage_cabinet", "Storage Cabinet", "Armoire de rangement", "Schrank", "Armadio");
        Add("table", "Table", "Table", "Tisch", "Tavolo");
        Add("bathtub", "Bathtub", "Baignoire", "Badewanne", "Vasca da bagno");
        Add("fireplace", "Fireplace", "Cheminée", "Kamin", "Camino");
        Add("microwave", "Microwave", "Micro-ondes", "Mikrowelle", "Microonde");
        Add("oven", "Oven", "Four", "Ofen", "Forno");
        Add("refrigerator", "Refrigerator", "Réfrigérateur", "Kühlschrank", "Frigorifero");
        Add("screen", "Screen", "Écran", "Bildschirm", "Schermo");
        Add("sink", "Sink", "Évier", "Waschbecken", "Lavandino");
        Add("tap", "Tap", "Robinet", "Wasserhahn", "Rubinetto");
        Add("toaster", "Toaster", "Grille-pain", "Toaster", "Tostapane");
        Add("toilet", "Toilet", "Toilette", "Toilette", "Toilette");

        // Objects & Miscellaneous
        Add("balloon", "Balloon", "Ballon", "Ballon", "Palloncino");
        Add("barrel", "Barrel", "Tonneau", "Fass", "Barile");
        Add("book", "Book", "Livre", "Buch", "Libro");
        Add("bottle", "Bottle", "Bouteille", "Flasche", "Bottiglia");
        Add("bowl", "Bowl", "Bol", "Schüssel", "Ciotola");
        Add("box", "Box", "Boîte", "Kiste", "Scatola");
        Add("camera", "Camera", "Appareil photo", "Kamera", "Macchina fotografica");
        Add("candle", "Candle", "Bougie", "Kerze", "Candela");
        Add("cannon", "Cannon", "Canon", "Kanone", "Cannone");
        Add("chopsticks", "Chopsticks", "Baguettes", "Essstäbchen", "Bacchette");
        Add("clock", "Clock", "Horloge", "Uhr", "Orologio");
        Add("coin", "Coin", "Pièce de monnaie", "Münze", "Moneta");
        Add("computer_keyboard", "Computer Keyboard", "Clavier d'ordinateur", "Computertastatur", "Tastiera del computer");
        Add("computer_mouse", "Computer Mouse", "Souris d'ordinateur", "Computermaus", "Mouse del computer");
        Add("cooking_pan", "Cooking Pan", "Poêle", "Pfanne", "Padella");
        Add("cup", "Cup", "Tasse", "Tasse", "Tazza");
        Add("curtain", "Curtain", "Rideau", "Vorhang", "Tenda");
        Add("doll", "Doll", "Poupée", "Puppe", "Bambola");
        Add("flowerpot", "Flowerpot", "Pot de fleurs", "Blumentopf", "Vaso di fiori");
        Add("fork", "Fork", "Fourchette", "Gabel", "Forchetta");
        Add("hair_dryer", "Hair Dryer", "Sèche-cheveux", "Haartrockner", "Asciugacapelli");
        Add("headphones", "Headphones", "Écouteurs", "Kopfhörer", "Cuffie");
        Add("jug", "Jug", "Cruche", "Krug", "Brocca");
        Add("knife", "Knife", "Couteau", "Messer", "Coltello");
        Add("lamp", "Lamp", "Lampe", "Lampe", "Lampada");
        Add("laptop", "Laptop", "Ordinateur portable", "Laptop", "Computer portatile");
        Add("microphone", "Microphone", "Microphone", "Mikrofon", "Microfono");
        Add("pen", "Pen", "Stylo", "Stift", "Penna");
        Add("phone", "Phone", "Téléphone", "Telefon", "Telefono");
        Add("pillow", "Pillow", "Oreiller", "Kissen", "Cuscino");
        Add("plate", "Plate", "Assiette", "Teller", "Piatto");
        Add("potted_plant", "Potted Plant", "Plante en pot", "Topfpflanze", "Pianta in vaso");
        Add("remote", "Remote", "Télécommande", "Fernbedienung", "Telecomando");
        Add("scissors", "Scissors", "Ciseaux", "Schere", "Forbici");
        Add("snowman", "Snowman", "Bonhomme de neige", "Schneemann", "Pupazzo di neve");
        Add("spoon", "Spoon", "Cuillère", "Löffel", "Cucchiaio");
        Add("teapot", "Teapot", "Théière", "Teekanne", "Teiera");
        Add("teddy_bear", "Teddy Bear", "Ours en peluche", "Teddybär", "Orsacchiotto");
        Add("tin_can", "Tin Can", "Boîte de conserve", "Dose", "Lattina");
        Add("toothbrush", "Toothbrush", "Brosse à dents", "Zahnbürste", "Spazzolino da denti");
        Add("toy", "Toy", "Jouet", "Spielzeug", "Giocattolo");
        Add("watch", "Watch", "Montre", "Uhr", "Orologio");
        Add("wine_glass", "Wine Glass", "Verre à vin", "Weinglas", "Bicchiere da vino");

        // Plants & Flowers
        Add("flower", "Flower", "Fleur", "Blume", "Fiore");
        Add("rose", "Rose", "Rose", "Rose", "Rosa");
        Add("sunflower", "Sunflower", "Tournesol", "Sonnenblume", "Girasole");

        // Animals
        Add("animal", "Animal", "Animal", "Tier", "Animale");
        Add("bird", "Bird", "Oiseau", "Vogel", "Uccello");
        Add("parrot", "Parrot", "Perroquet", "Papagei", "Pappagallo");
        Add("water_bird", "Water Bird", "Oiseau aquatique", "Wasservogel", "Uccello acquatico");
        Add("butterfly", "Butterfly", "Papillon", "Schmetterling", "Farfalla");
        Add("insect", "Insect", "Insecte", "Insekt", "Insetto");
        Add("dolphin", "Dolphin", "Dauphin", "Delfin", "Delfino");
        Add("fish", "Fish", "Poisson", "Fisch", "Pesce");
        Add("goldfish", "Goldfish", "Poisson rouge", "Goldfisch", "Pesce rosso");
        Add("jellyfish", "Jellyfish", "Méduse", "Qualle", "Medusa");
        Add("seal", "Seal", "Phoque", "Robbe", "Foca");
        Add("shellfish", "Shellfish", "Crustacé", "Schalentier", "Crostaceo");
        Add("whale", "Whale", "Baleine", "Wal", "Balena");
        Add("alpaca", "Alpaca", "Alpaga", "Alpaka", "Alpaca");
        Add("bear", "Bear", "Ours", "Bär", "Orso");
        Add("big_cat", "Big Cat", "Grand félin", "Großkatze", "Grande felino");
        Add("camel", "Camel", "Chameau", "Kamel", "Cammello");
        Add("cat", "Cat", "Chat", "Katze", "Gatto");
        Add("cow", "Cow", "Vache", "Kuh", "Mucca");
        Add("crocodile", "Crocodile", "Crocodile", "Krokodil", "Coccodrillo");
        Add("deer", "Deer", "Cerf", "Hirsch", "Cervo");
        Add("dog", "Dog", "Chien", "Hund", "Cane");
        Add("elephant", "Elephant", "Éléphant", "Elefant", "Elefante");
        Add("frog", "Frog", "Grenouille", "Frosch", "Rana");
        Add("giraffe", "Giraffe", "Girafe", "Giraffe", "Giraffa");
        Add("hippopotamus", "Hippopotamus", "Hippopotame", "Nilpferd", "Ippopotamo");
        Add("horse", "Horse", "Cheval", "Pferd", "Cavallo");
        Add("kangaroo", "Kangaroo", "Kangourou", "Känguru", "Canguro");
        Add("panda", "Panda", "Panda", "Panda", "Panda");
        Add("pig", "Pig", "Cochon", "Schwein", "Maiale");
        Add("polar_bear", "Polar Bear", "Ours polaire", "Eisbär", "Orso polare");
        Add("rabbit", "Rabbit", "Lapin", "Kaninchen", "Coniglio");
        Add("reptile", "Reptile", "Reptile", "Reptil", "Rettile");
        Add("rhinoceros", "Rhinoceros", "Rhinocéros", "Nashorn", "Rinoceronte");
        Add("sheep", "Sheep", "Mouton", "Schaf", "Pecora");
        Add("squirrel", "Squirrel", "Écureuil", "Eichhörnchen", "Scoiattolo");
        Add("turtle", "Turtle", "Tortue", "Schildkröte", "Tartaruga");
        Add("zebra", "Zebra", "Zèbre", "Zebra", "Zebra");

        // Save the asset
        EditorUtility.SetDirty(_targetDatabase);
        AssetDatabase.SaveAssets();

        // Validate count
        int actualCount = GetEntryCount();
        _isPopulated = true;

        if (actualCount == 206)
        {
            Debug.Log($"[TranslationPopulator] ✓ SUCCESS! Populated database with {actualCount} entries × 3 languages = {actualCount * 3} translations");
            EditorUtility.DisplayDialog(
                "Success!",
                $"Translation database populated successfully!\n\n" +
                $"Entries: {actualCount}\n" +
                $"Total translations: {actualCount * 3} (FR, DE, IT)\n\n" +
                "Your app is now fully offline!",
                "OK"
            );
        }
        else
        {
            Debug.LogWarning($"[TranslationPopulator] WARNING! Expected 206 entries but got {actualCount}");
            EditorUtility.DisplayDialog(
                "Partial Success",
                $"Database populated but entry count is unexpected.\n\n" +
                $"Expected: 206\n" +
                $"Actual: {actualCount}\n\n" +
                "Check the Console for details.",
                "OK"
            );
        }
    }

    private void Add(string objectClass, string english, string french, string german, string italian)
    {
        _targetDatabase.AddTranslation(objectClass, english, french, german, italian);
    }

    private void ClearDatabase()
    {
        // Use reflection to clear the private _translations list
        var field = typeof(TranslationDatabaseSO).GetField("_translations",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

        if (field != null)
        {
            var list = field.GetValue(_targetDatabase) as System.Collections.IList;
            if (list != null)
            {
                list.Clear();
                EditorUtility.SetDirty(_targetDatabase);
                Debug.Log("[TranslationPopulator] Cleared existing entries");
            }
        }
    }

    private int GetEntryCount()
    {
        var field = typeof(TranslationDatabaseSO).GetField("_translations",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

        if (field != null)
        {
            var list = field.GetValue(_targetDatabase) as System.Collections.IList;
            if (list != null)
            {
                return list.Count;
            }
        }

        return 0;
    }
}
