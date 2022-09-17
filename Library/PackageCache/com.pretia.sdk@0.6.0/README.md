# Pretia SDK

Pretia SDK is a part of the Pretia AR Cloud platform to help create shared AR experiences easily. Please head over to [https://docs.developers.pretiaar.com](https://docs.developers.pretiaar.com) to learn more about it.

## Pretia SDK Quick Start<a name="pretia-sdk-quick-start"></a>

### Import Pretia SDK<a name="import-pretia-sdk"></a>

Pretia SDK is a upm-based Unity package stored in [https://npmjs.com](https://npmjs.com). In order to import it, you will need to add the following to the `scopedRegistries` section in the manifest file (`Packages/manifest.json`):

```json
{
  "name": "Pretia Technologies",
  "url": "https://registry.npmjs.com",
  "scopes": [ "com.pretia" ]
}
```

And you will also need to add `com.pretia.sdk` to the dependency:

```json
"com.pretia.sdk": "0.5.0"
```

This is how your manifest file should look like after the changes:
```json
{
  "dependencies": {
    "com.pretia.sdk": "0.5.0",
    ...
  },
  "scopedRegistries": [
    {
      "name": "Pretia Technologies",
      "url": "https://registry.npmjs.com",
      "scopes": [ "com.pretia" ]
    }
  ]
}
```

Save the file and open your Unity project. It should import Pretia SDK together with all of its dependencies.