# Certificate for staging :scroll: 

When testing the mobile app, one should always use the staging environment. Xamarin has some problems with that due to the certificate staging uses, so you need to do some preparations before testing the application.

## Integration tests :computer:

For integration tests we [disabled certificate checks entirely](https://github.com/toggl/mobileapp/blob/develop/Toggl.Ultrawave.Tests.Integration/Helper/User.cs#L22), since they are a controlled environment.

## Android App :robot:

1. Request the certificate link on #mobile-dev (it's pinned, but you can always ask if you can't find it).
2. Open it, give it a name and click install. 
3. There is no step 3. Android is simple.

## iOS App :iphone:

1. Request the certificate link on #mobile-dev (it's pinned, but you can always ask if you can't find it).
2. When you open it, you will see [this screen](https://user-images.githubusercontent.com/7688727/29566049-ca4995f4-871e-11e7-938d-41ddbbd38cd0.png).
3. Click install, pretend you read the warning and then click install again. When the prompt appears, click install a third time and question Apple's UX team.
4. If you followed it all correctly, you will see [this screen](https://user-images.githubusercontent.com/7688727/29566109-fb0f63ee-871e-11e7-8a4a-762e6c9a0a0a.png). Click done to close the settings.
5. Navigate back to the home screen and open the settings app.
6. Click General -> About -> Certificate Trust Settings. You shall see [this](https://user-images.githubusercontent.com/7688727/29566227-6c3ae746-871f-11e7-8287-fca4429ed1ac.png).
7. Toggle (yes, with an `e` at the end) the switch to enable the `Toggl Root CA` certificate. Click `Continue` on the prompt that will show up.

The app now works on staging :tada:
