@REM   .\publish.bat $(git rev-parse HEAD)
 cd BE
 
 IF NOT "%2" == "FE" (
 docker build -f Dockerfile --tag lunale/ordboxcompany:api-%1 --tag lunale/ordboxcompany:api-latest .
 docker push lunale/ordboxcompany:api-%1
  docker push lunale/ordboxcompany:api-latest
 )

 cd ..\FE
 if not "%2" == "BE" (
 docker build -f Dockerfile --tag lunale/ordboxcompany:front-%1 --tag lunale/ordboxcompany:front-latest .
 docker push lunale/ordboxcompany:front-%1
 docker push lunale/ordboxcompany:front-latest
  )