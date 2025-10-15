# SDR-LAB1
Dataset-ul ales reprezinta o versiune prelucrata a fisierului public - 30000 Spotify Songs luat de pe Kaggle - https://www.kaggle.com/datasets/joebeachcapital/30000-spotify-songs?resource=download
Datele originale provin din API-ul Spotify si contin metadate despre melodii din mai multe genuri muzicale.
Am pastrat din datasetul original doar urmatoarele coloane:
-	track_id – identificator unic pentru fiecare melodie
-	track_name – numele melodiei
-	track_artist – numele artistului
-	track_popularity – scor de popularitate (0–100)
-	track_album_name – numele albumului din care face parte melodia
-	track_album_release_date – data lansarii albumului
-	danceability – descrie cat de potrivita este piesa pentru dans
si am salvat noul data set in spotify_songs_cleaned.csv.
Am folosit acest data set pentru:
•	Definirea proprietatilor itemilor : song -string, artist - string, album-string, popularity - int, danceability- double.
•	Setarea valorilor item-urilor, adica adaugarea melodiilor in baza de date Recombee.
