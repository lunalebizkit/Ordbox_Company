 REM .\publish.bat $(git rev-parse HEAD)
 cd BE
 
 IF NOT "%2" == "front" (
 docker build --no-cache -f Dockerfile --tag lunale/ordbox:api-%1 --tag lunale/ordbox:api-latest .
 docker push lunale/ordbox:api-%1
 docker push lunale/ordbox:api-latest
 )

@REM  cd ..\Front
@REM  IF NOT "%2" == "api" (
@REM  docker build -f Dockerfile --tag lunale/ordbox:front-%1 --tag lunale/ordbox:front-latest .
@REM  docker push lunale/ordbox:front-%1
@REM  docker push lunale/ordbox:front-latest
@REM  )