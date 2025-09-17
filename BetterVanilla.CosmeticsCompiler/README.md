# BetterVanilla Cosmetics Compiler

Un outil simple pour créer et compiler des cosmétiques personnalisés pour le mod Among Us BetterVanilla.

## 🚀 Utilisation rapide

### Méthode automatique (recommandée)

1. **Placez le fichier exécutable** `BetterVanilla.CosmeticsCompiler.exe` dans le dossier de votre choix
2. **Créez un fichier texte** nommé `cosmetics-bundle-config.txt` dans le même dossier que l'exécutable
3. **Remplissez le fichier** avec vos commandes (voir exemples ci-dessous)
4. **Double-cliquez** sur l'exécutable (il lira automatiquement le fichier de configuration)

### Méthode manuelle (pour utilisateurs avancés)

Ouvrez une invite de commande dans le dossier contenant l'exécutable et utilisez les commandes directement.

## 📝 Format du fichier `cosmetics-bundle-config.txt`

Le fichier doit contenir vos commandes, **une instruction par ligne**. Laissez une **ligne vide** entre chaque commande complète.

### Exemple complet :

```
create-hat-spritesheet
--name
"Mon Chapeau"
--author
"Mon Pseudo"
--output
"C:\Mon\Dossier\Output"
--main-resource
"C:\Mon\Dossier\Images\chapeau.png"

create-visor-spritesheet
--name
"Mes Lunettes"
--author
"Mon Pseudo"
--output
"C:\Mon\Dossier\Output"
--main-resource
"C:\Mon\Dossier\Images\lunettes.png"

bundle
--compression
--output
"C:\Mon\Dossier\Final\cosmetics.bundle"
--hats
"C:\Mon\Dossier\Output\Mon Chapeau.spritesheet.json"
--visors
"C:\Mon\Dossier\Output\Mes Lunettes.spritesheet.json"
```

## 🎩 Créer un chapeau (Hat)

```
create-hat-spritesheet
--name
"Nom du chapeau"
--author
"Votre nom"
--output
"Dossier de sortie"
--main-resource
"Chemin vers l'image principale"
```

### Options supplémentaires pour les chapeaux :

- `--adaptive` : Chapeau qui s'adapte aux couleurs du joueur
- `--bounce` : Chapeau qui rebondit
- `--no-visors` : Empêche le port de visières avec ce chapeau
- `--flip-resource` : Image pour la vue de côté (flip horizontal de main-resource)
- `--back-resource` : Image pour la vue arrière
- `--back-flip-resource` : Image arrière pour la vue de côté (flip horizontal de back-resource)
- `--climb-resource` : Image pendant l'escalade
- `--front-animation-frames` : Liste des images pour l'animation frontale
- `--back-animation-frames` : Liste des images pour l'animation arrière

### Exemple avec toutes les options :

```
create-hat-spritesheet
--name
"Mon Chapeau Animé"
--author
"Mon Pseudo"
--adaptive
--bounce
--output
"C:\Mon\Dossier\Output"
--main-resource
"C:\Images\chapeau_main.png"
--flip-resource
"C:\Images\chapeau_flip.png"
--back-resource
"C:\Images\chapeau_dos.png"
--back-flip-resource
"C:\Images\chapeau_dos_flip.png"
--climb-resource
"C:\Images\chapeau_escalade.png"
--front-animation-frames
"C:\Images\Animation\frame1.png"
"C:\Images\Animation\frame2.png"
"C:\Images\Animation\frame3.png"
"C:\Images\Animation\frame3.png"
"C:\Images\Animation\frame3.png"
--back-animation-frames
"C:\Images\AnimationDos\frame1.png"
"C:\Images\AnimationDos\frame2.png"
```

## 👓 Créer une visière (Visor)

```
create-visor-spritesheet
--name
"Nom de la visière"
--author
"Votre nom"
--output
"Dossier de sortie"
--main-resource
"Chemin vers l'image principale"
```

### Options supplémentaires pour les visières :

- `--adaptive` : Visière qui s'adapte aux couleurs du joueur
- `--behind-hat` : Visière portée derrière le chapeau
- `--left-resource` : Image pour la vue de côté gauche
- `--climb-resource` : Image pendant l'escalade
- `--floor-resource` : Image quand le joueur est au sol
- `--front-animation-frames` : Liste des images pour l'animation frontale

### Exemple avec toutes les options :

```
create-visor-spritesheet
--name
"Mes Lunettes Animées"
--author
"Mon Pseudo"
--adaptive
--behind-hat
--output
"C:\Mon\Dossier\Output"
--main-resource
"C:\Images\lunettes_main.png"
--left-resource
"C:\Images\lunettes_gauche.png"
--climb-resource
"C:\Images\lunettes_escalade.png"
--floor-resource
"C:\Images\lunettes_sol.png"
--front-animation-frames
"C:\Images\LunettesAnim\blink1.png"
"C:\Images\LunettesAnim\blink2.png"
"C:\Images\LunettesAnim\blink1.png"
"C:\Images\LunettesAnim\blink1.png"
"C:\Images\LunettesAnim\blink1.png"
```

## 📦 Créer un pack final (Bundle)

```
bundle
--output
"Chemin vers le fichier .bundle final"
--hats
"Chemin vers chapeau1.spritesheet.json"
"Chemin vers chapeau2.spritesheet.json"
--visors
"Chemin vers visiere1.spritesheet.json"
```

### Options supplémentaires :

- `--compression` : Compresse le fichier final pour économiser l'espace

### 🎮 Tester vos cosmétiques en jeu

Pour utiliser vos cosmétiques directement dans Among Us avec BetterVanilla :

1. **Placez votre fichier .bundle** dans le dossier :
   ```
   C:\Users\[VotreNom]\AppData\LocalLow\Innersloth\Among Us\EnoPM\BetterVanilla.AmongUs\Cosmetics\LocalBundles
   ```

2. **Redémarrez Among Us** - Les nouveaux cosmétiques n'apparaissent qu'après un redémarrage complet du jeu

3. **Accédez aux cosmétiques** dans le menu du jeu

### 💡 Astuce : Génération directe
Pour plus de simplicité, vous pouvez générer directement dans ce dossier :

```
bundle
--compression
--output
"C:\Users\[VotreNom]\AppData\LocalLow\Innersloth\Among Us\EnoPM\BetterVanilla.AmongUs\Cosmetics\LocalBundles\mon-pack.bundle"
--hats
"C:\Mon\Dossier\Output\chapeau1.spritesheet.json"
--visors
"C:\Mon\Dossier\Output\visiere1.spritesheet.json"
```

**⚠️ Remplacez `[VotreNom]` par votre nom d'utilisateur Windows**

## 🎬 Guide des animations

### Comment fonctionnent les animations

Les animations sont créées en listant plusieurs images dans l'ordre de lecture. Chaque image représente une frame d'animation.

### Animation IDLE (statique)

Pour créer une pause dans l'animation ou garder une image plus longtemps, **répétez la même image plusieurs fois** :

```
--front-animation-frames
"image1.png"
"image2.png"
"image2.png"    ← Image affichée plus longtemps
"image2.png"    ← pour créer une pause
"image3.png"
```

### Exemples d'animations

#### Animation de clignotement (visière)
```
--front-animation-frames
"yeux_ouverts.png"
"yeux_ouverts.png"
"yeux_ouverts.png"
"yeux_fermes.png"
"yeux_ouverts.png"
```

#### Animation rebondissante (chapeau)
```
--front-animation-frames
"position_base.png"
"position_haute.png"
"position_base.png"
"position_base.png"   ← Pause en position normale
"position_base.png"
```

#### Animation continue
```
--front-animation-frames
"frame1.png"
"frame2.png"
"frame3.png"
"frame4.png"
"frame3.png"
"frame2.png"    ← Retour en arrière pour une boucle fluide
```

### Conseils pour les animations

- **Vitesse** : Plus vous répétez une image, plus elle reste affichée longtemps
- **Fluidité** : Utilisez des transitions graduelles entre les frames
- **Boucle** : Pensez à ce que la dernière frame se connecte bien avec la première
- **Simplicité** : Les animations subtiles sont souvent plus agréables

## 💡 Conseils pratiques

### Chemins de fichiers
- **Utilisez toujours des guillemets** autour des chemins contenant des espaces
- **Utilisez des barres obliques inverses** sur Windows : `C:\Dossier\fichier.png`

### Organisation
- Créez un dossier dédié pour vos cosmétiques
- Placez toutes vos images source dans un sous-dossier
- Définissez un dossier de sortie pour les fichiers générés

### Format des images
- **PNG recommandé** pour la transparence
- **Taille recommandée** : 300x375 pixels
- **Fond transparent** pour un meilleur rendu

## ⚠️ Problèmes courants

### L'exécutable ne fait rien
- Vérifiez que le fichier `cosmetics-bundle-config.txt` existe dans le même dossier
- Vérifiez que le fichier n'est pas vide
- Ouvrez une invite de commande pour voir les messages d'erreur

### Erreur "File not found"
- Vérifiez que tous les chemins dans votre fichier de configuration existent
- Utilisez des guillemets autour des chemins avec des espaces

### Images mal rendues
- Vérifiez que vos images sont au format PNG
- Assurez-vous que la transparence est correctement définie
- Testez avec des images plus petites si nécessaire

## 📁 Structure de dossier recommandée

```
Mon Projet Cosmétiques/
├── BetterVanilla.CosmeticsCompiler.exe
├── cosmetics-bundle-config.txt
├── Images Sources/
│   ├── chapeau1.png
│   ├── chapeau2.png
│   └── visiere1.png
├── Output/
│   ├── chapeau1.spritesheet.json
│   ├── chapeau2.spritesheet.json
│   └── visiere1.spritesheet.json
└── Final/
    └── mon-pack-cosmetics.bundle
```

---

**Besoin d'aide ?** Ce programme génère des fichiers que vous pouvez utiliser avec le mod BetterVanilla pour Among Us. Consultez la documentation de BetterVanilla pour plus d'informations sur l'installation des cosmétiques.