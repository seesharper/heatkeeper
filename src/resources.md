https://www.qamilestone.com/post/end-to-end-steps-to-set-up-influxdb-grafana-using-docker-compose

https://thomaslevesque.com/2015/06/28/how-to-retrieve-dates-as-utc-in-sqlite/

https://restfulapi.net/http-methods/#post

https://snippet-generator.app/

http://jasonwatmore.com/post/2018/08/14/aspnet-core-21-jwt-authentication-tutorial-with-example-api

https://github.com/aspnet/Identity/blob/master/src/Core/PasswordHasher.cs

https://www.jerriepelser.com/blog/using-roles-with-the-jwt-middleware/

https://crackstation.net/hashing-security.htm

https://github.com/grandchamp/Identity.Dapper

https://andrewlock.net/exploring-the-asp-net-core-identity-passwordhasher/

https://stackoverflow.com/questions/46793216/whats-the-user-parameter-to-passwordhashers-methods-used-for

https://docs.microsoft.com/en-us/aspnet/core/fundamentals/configuration/options?view=aspnetcore-2.2

https://stackoverflow.com/questions/38340078/how-to-decode-jwt-token

https://tahirnaushad.com/2017/09/04/consuming-asp-net-core-2-0-web-api-using-httpclient/

https://fullstackmark.com/post/13/jwt-authentication-with-aspnet-core-2-web-api-angular-5-net-core-identity-and-facebook-login

https://wildermuth.com/2017/08/19/Two-AuthorizationSchemes-in-ASP-NET-Core-2

https://docs.microsoft.com/en-us/aspnet/core/security/authentication/cookie?view=aspnetcore-2.2

https://auth0.com/blog/using-json-web-tokens-as-api-keys/

https://stackoverflow.com/questions/26739167/jwt-json-web-token-automatic-prolongation-of-expiration

https://stackoverflow.com/questions/9220432/http-401-unauthorized-or-403-forbidden-for-a-disabled-user

https://www.strathweb.com/2018/07/centralized-exception-handling-and-request-validation-in-asp-net-core/

https://jeroenhildering.com/2016/11/24/mapping-exceptions-to-http-responses-with-net-core/


https://docs.influxdata.com/influxdb/cloud/query-data/common-queries/compare-values/


https://stackoverflow.com/questions/70650711/libsqlite-interop-not-found-mac-os-m1


```
test2 = from(bucket: "None")
  |> range(start: v.timeRangeStart, stop: v.timeRangeStop)
  |> filter(fn: (r) => r["_measurement"] == "ElectricalPricePerkWh")
  |> filter(fn: (r) => r["_field"] == "PriceInNOK")
  |> mean()
  |> findColumn(
      fn: (key) => key._field == "PriceInNOK",
      column: "_value"
  )
  

from(bucket: "None")
  |> range(start: v.timeRangeStart, stop: v.timeRangeStop)
  |> filter(fn: (r) => r["_measurement"] == "CumulativePowerImport")
  |> filter(fn: (r) => r["_field"] == "value")
  |> difference()
  |> map(fn: (r) => ({    
    _time: r._time,
    _value: r._value * test2[0],
    field_name: r.field_name
  }))
  |> yield(name: "first")  
```


https://www.homeautomationguy.io/docker-tips/configuring-the-mosquitto-mqtt-docker-container-for-use-with-home-assistant/


https://www.instructables.com/Tasmotized-NodeMCU-8CH-Sonoff-Relay/

https://lastminuteengineers.com/esp8266-pinout-reference/

```
from(bucket: "None")
  |> range(start: v.timeRangeStart, stop: v.timeRangeStop)
  |> filter(fn: (r) => r["_measurement"] == "CumulativePowerImport")
  |> filter(fn: (r) => r["_field"] == "value")
  |> difference()
  |> map(fn: (r) => ({    
    _time: r._time,
    _value: r._value * 1.0,
    field_name: r.field_name
  }))
  |> yield(name: "first")


  from(bucket: "None")
  |> range(start: v.timeRangeStart, stop: v.timeRangeStop)
  |> filter(fn: (r) => r["_measurement"] == "ElectricalPricePerkWh")
  |> filter(fn: (r) => r["_field"] == "PriceInNOK")
  |> mean()
  |> yield(name: "averagePrice")
 ```

 
 https://rafaelldi.blog/posts/open-telemetry-in-dotnet/


 https://resources.pcb.cadence.com/blog/2021-how-does-temperature-hysteresis-work


 https://github.com/vvatelot/mosquitto-docker-compose



 https://linuxize.com/post/how-to-setup-passwordless-ssh-login/


 https://linuxize.com/post/how-to-set-and-list-environment-variables-in-linux/  


Shelly Plug S
 https://www.youtube.com/watch?v=_huPdu7paYw


FINDING YOUR RASPBERRY PI ON THE NETWORK FROM A MAC

 https://spellfoundry.com/docs/finding-your-raspberry-pi-on-the-network-from-a-mac/


Clearing retained message on mosquitto 

```
mosquitto_pub -t "ShellyHT_Stue_Hytta/online" --username heatkeeper --pw overintermoduluasjonsforvregning -r -m ""
```


https://blogs.sap.com/2019/12/17/understanding-containers-part-03-one-shot-containers/




https://devops.stackexchange.com/questions/4540/how-to-change-the-owner-of-volume-directory-in-dockerfile

https://duncanlock.net/blog/2023/05/23/using-git-hashes-in-vite-vuejs/

https://stackoverflow.com/questions/77647463/sveltekit-webapp-to-pwa-progressive-web-app-how-to-do-it-in-the-most-simple-w

https://www.danielzotti.it/blog/notifications-in-browsers



## Create a certificate

```
sudo certbot certonly --standalone
```


This outputted 

```
Saving debug log to /var/log/letsencrypt/letsencrypt.log
Please enter the domain name(s) you would like on your certificate (comma and/or
space separated) (Enter 'c' to cancel): heatkeeper.no
Requesting a certificate for heatkeeper.no

Successfully received certificate.
Certificate is saved at: /etc/letsencrypt/live/heatkeeper.no/fullchain.pem
Key is saved at:         /etc/letsencrypt/live/heatkeeper.no/privkey.pem
This certificate expires on 2024-06-15.
These files will be updated when the certificate renews.
Certbot has set up a scheduled task to automatically renew this certificate in the background.

- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
If you like Certbot, please consider supporting our work by:
 * Donating to ISRG / Let's Encrypt:   https://letsencrypt.org/donate
 * Donating to EFF:                    https://eff.org/donate-le
- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
```