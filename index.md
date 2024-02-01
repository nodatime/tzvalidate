---
title: tzvalidate files
---

# tzvalidate files

This is a potentially-temporary home for files created by
the [tzvalidate](https://github.com/nodatime/tzvalidate)
project, which aims to provide appropriate files so that other
projects consuming the raw [IANA time zone data](http://iana.org/time-zones)
can check that they are interpreting it in the same was as `zic`,
which is deemed to be canonical.

The hex string after each link is the SHA-256 hash of the *body* of
the tzvalidate file. This information is also available as a text file
per release, with a name of `tzdata(release)-sha256.txt`. For example,
the URL for the 2016d release body SHA-256 is
https://nodatime.github.io/tzvalidate/tzdata2016d-sha256.txt. Note that
this is *only* the SHA-256 of the body, not of the whole file. (Its
purpose is to enable automatic validation of other generators without
having to download the whole generated file.)

Eventually I hope to have a better home for the files, and the format
may change over time too - see the `tzvalidate` project page for more
information. At the moment, I'm taking a position of "something is better than nothing."

Data for every release from 93g onwards is hosted here, except 2005l which gave
me errors when running `zic`. Each release is in a separate zip file which
just consists of a single text file. When new data is available, it
is built semi-automatically (in that it's automated, but needs a
nudge to start it.

Data up to 2016d inclusive was generated with the 2016d code. 
After that, the data is generated with the code from the corresponding release.

[//]: # Insert here
- [2024a](tzdata2024a-tzvalidate.zip): 9d0943a77ab0f1abe228dc5d9e455ff94e4596221696ff11391b74989941115d
- [2023d](tzdata2023d-tzvalidate.zip): c2d7643c9113c1a974e5e40c69bf50dd0f18f63957038e6b5218f6ef686ba984
- [2023c](tzdata2023c-tzvalidate.zip): 87bad71ef2ae03315aa32ba8f669c3377b31e66927ddb7f6c9a6ae27243c616a
- [2023b](tzdata2023b-tzvalidate.zip): 409d8d2fd35344e4c9fa8795024f2df7a28a31fcf730923e2c9a9bcf082c7ffd
- [2023a](tzdata2023a-tzvalidate.zip): 87bad71ef2ae03315aa32ba8f669c3377b31e66927ddb7f6c9a6ae27243c616a
- [2022g](tzdata2022g-tzvalidate.zip): 979f6fe7629f2fd48f332d872dbb3478bf22c88d0b3aad4bcad99a98c3b03095
- [2022f](tzdata2022f-tzvalidate.zip): dfa83dc447b3551bcb80280c6e24db88ce02e5fad4a2fdce12dd54759282dc44
- [2022e](tzdata2022e-tzvalidate.zip): 5f9758b18afdaeced01e48f1fac0d64f4b3a9873b90ca154985d5b4cda7641ce
- [2022d](tzdata2022d-tzvalidate.zip): 87471cf130316dd7a9dad77247fb32b369d227ea414bd08c1d82778553477fac
- [2022c](tzdata2022c-tzvalidate.zip): d9bacc1efb4d58ed734e28c4dc12082eb67532925fe2ae66c6408b71f69a0904
- [2022b](tzdata2022b-tzvalidate.zip): d9bacc1efb4d58ed734e28c4dc12082eb67532925fe2ae66c6408b71f69a0904
- [2022a](tzdata2022a-tzvalidate.zip): a7ffab76f47d718a920326c1a521babfc83713a790180c1fd0396e8c3bca38a7
- [2021e](tzdata2021e-tzvalidate.zip): fb69963e6bcce2e41a81373b98ef1d8daed2727e0b56c424013a94ee912e1ead
- [2021d](tzdata2021d-tzvalidate.zip): 4ba17b1e3720f462e9a2386f9ea73002c4e6bad9525ac32d6b3016a8839eb248
- [2021c](tzdata2021c-tzvalidate.zip): 024ee9324dc81affaa883e0ed690f349e354cd0db2ae2324bf11b76f93bad038
- [2021b](tzdata2021b-tzvalidate.zip): 13fa013152ee4794521cc8d12d8dd2ce3c08450aeeebf38e44df48cf0f0bb28a
- [2021a](tzdata2021a-tzvalidate.zip): c5550135b8394d87149ee0d82c590ed0665fc0e5a16227f2de3d57b092c5e550
- [2020e](tzdata2020e-tzvalidate.zip): 4ac032954517588d8cebbf3b2a3d28668dddab5ba59067a3ec615c0bd8597723
- [2020d](tzdata2020d-tzvalidate.zip): 3326fc01a4328fdf913cc88075bb43500ef3b1352b54c1a4ea60c6656181d07f
- [2020c](tzdata2020c-tzvalidate.zip): b453dfc1581fb3630bcfe636c77ce2a1f222dce4fcaf53b47d1f6361993a195b
- [2020b](tzdata2020b-tzvalidate.zip): ad70978770691d6a4a46fb96102979e024936444787ba9684f32018032bad5fa
- [2020a](tzdata2020a-tzvalidate.zip): 7cedd24b16b406b1b70ab67c4f89d9db00935fd0a496721cadc9c62dd10453e3
- [2019c](tzdata2019c-tzvalidate.zip): bde03efbc2dfb579d1b2e15768b8bfe9947c369eda704b880bb802ce725eaed1
- [2019b](tzdata2019b-tzvalidate.zip): caeb3a686d76736bae6469ad8982819599e54535f17f1de44f91493e1a839f33
- [2019a](tzdata2019a-tzvalidate.zip): cf9c2c3b55781d8741458473f2fdfd7bd3ee2df6f72a4223089de80ab18d41bb
- [2018i](tzdata2018i-tzvalidate.zip): e3cc760538424212aa6e42f98f39146e7d3d54199d4ebd50099ab64b45dc7965
- [2018h](tzdata2018h-tzvalidate.zip): 3520578827bce937d819797bc7248a2c2180829c6bbb7e63b1e3d4367404c378
- [2018g](tzdata2018g-tzvalidate.zip): ce2808123ca9d534095df299ef06a5eb8f36ad022d67eefa6e088db0b1951c96
- [2018f](tzdata2018f-tzvalidate.zip): f8282b240e6f0579ac4c7b00eace70ac4f74154a44c9339eb9cf754993a32a12
- [2018e](tzdata2018e-tzvalidate.zip): b634fd443739e925e763d7b81a84bf592f7df554bf006db74f739db53175aa51
- [2018d](tzdata2018d-tzvalidate.zip): 671c0f7d7bce45d600b26ece7043c9b54253f1824f779add00e6af09dc97f16f
- [2018c](tzdata2018c-tzvalidate.zip): 61b58a957f9367330cd6a6d27bd468e3e27c79e4048a13d6ccb54e27444002f8
- [2018b](tzdata2018b-tzvalidate.zip): dcba7d2bbe90544269686f31aba516eda1961ab80fc84c3962e93c26441013a7
- [2017c](tzdata2017c-tzvalidate.zip): f425da3a0d4a2836a6eae75425c022881f97a4f55371e538ce7a2f8bdc81b96c
- [2017b](tzdata2017b-tzvalidate.zip): eeffac6353908966e8ca50cae761ba98f1aec8a234ba48a0d557d71665a54ec9
- [2017a](tzdata2017a-tzvalidate.zip): 7acebd03be47e9e8b0c65cee833bfac9c3a78788fa941647b5a85976707f4137
- [2016j](tzdata2016j-tzvalidate.zip): 4a2ee801a37afa99b09dbc27984156cc1eb9747737035523ce85f66c324c9b73
- [2016i](tzdata2016i-tzvalidate.zip): b051ca8f785a9a582a500e774ca195f7a25f0449eac595fb59ba54b32036d8bd
- [2016h](tzdata2016h-tzvalidate.zip): b0ce8656d8c4f7feae1ed105aa579236f48f2ab3b7d67ccf460498b7849abb93
- [2016g](tzdata2016g-tzvalidate.zip): bae5c5c241009858ded41305c4622f1c6b9a9286539a3d0f4b7f693939d07a74
- [2016f](tzdata2016f-tzvalidate.zip): 8adf627bf973a163a8153ba66aa655d725402afad1533a915e354f37ae67a1b0
- [2016e](tzdata2016e-tzvalidate.zip): 68b83ad70fb4131803e37165ff641ce06d3d7e189efb8d781bdbdc0f21cde0c7
- [2016d](tzdata2016d-tzvalidate.zip): ad3478345eba4c8577c1084b7b525ff43a504f90b89e51d6d75b693bceb2d1ad
- [2016c](tzdata2016c-tzvalidate.zip): 3e96a190baaa7130115fad1d6ae9bbc39754d605c71e648fae31d1546a336af9
- [2016b](tzdata2016b-tzvalidate.zip): fbd8894d93c08effca52e2aa5f8c3835885416d0129db5236e08985714c41b5b
- [2016a](tzdata2016a-tzvalidate.zip): 7d7cdd3e373b515ec6da310e6c4d060a8a0e672b061d90af31c41902412d0106
- [2015g](tzdata2015g-tzvalidate.zip): f5c6714301f02926fa19f9f1e68070ba385a26b345f606876727e80e6eb761d2
- [2015f](tzdata2015f-tzvalidate.zip): c1c80ccd0a57dd2f6b513ed1671e385a7ed1803a9fe44ae92d5a1fb3c471a549
- [2015e](tzdata2015e-tzvalidate.zip): 42d6618d4c6260c3a242ce94b0d3dab587cc473842ecf5f9369a362e3f96c314
- [2015d](tzdata2015d-tzvalidate.zip): a1a76fec7d93a6155eb6dc2f644126e5569f48daa8176e2e952de26f0e804831
- [2015c](tzdata2015c-tzvalidate.zip): 7c1078b3d07b1209feb71a289d9c1dac50fc1cbe9dd9400c108a4582a8cd530e
- [2015b](tzdata2015b-tzvalidate.zip): b499776eeb837a462be107b6cb16c8bf6c8536d02ac9993ba1ddffaf9819f2c7
- [2015a](tzdata2015a-tzvalidate.zip): 99345887411cdc8df57194abdd94ae96e97d5f83c2a220e656106e8d9f5bc960
- [2014j](tzdata2014j-tzvalidate.zip): 2ad19b7b6b05017778097a98d230e47727c6803e2eabf3feabbbe333f244e63b
- [2014i](tzdata2014i-tzvalidate.zip): 3fda803ad2e42feee3519303e8ac725aed7564c938725307ce4a9ca5fcf86f94
- [2014h](tzdata2014h-tzvalidate.zip): 4416fe3a43efe0cccef7bf797bd6d1ac2e078975cdb1dcfba5c1f648430bab2a
- [2014g](tzdata2014g-tzvalidate.zip): 8536145c9f4d3c61d80a191950d2512f8c6c438ba353165c65b3d392fe64a7d4
- [2014f](tzdata2014f-tzvalidate.zip): b18acd8f24cf50ec19e52ee048b3752c10e39e598adef77769263138535c249a
- [2014e](tzdata2014e-tzvalidate.zip): 5a23183f52a4e963045c448ac5217b9644799ad7076fa8800506663f962d2adb
- [2014d](tzdata2014d-tzvalidate.zip): 9dfffb6728e5744fa8c9278cebfa4494bfe086609b08006c139db23dc9265814
- [2014c](tzdata2014c-tzvalidate.zip): 9dfffb6728e5744fa8c9278cebfa4494bfe086609b08006c139db23dc9265814
- [2014b](tzdata2014b-tzvalidate.zip): 9f6654f21c3dc830245098ef52dfc2e0282984b4b7826918e08d64abfb42b541
- [2014a](tzdata2014a-tzvalidate.zip): 86b3f5fab77bee82f05dceabda662faec8d6a2927c9740a467eb61ace00252d1
- [2013i](tzdata2013i-tzvalidate.zip): 8cbee642a8a8ec6285d090c8c22816dd81ffaeffca81ba71ddc60eea0aba01f0
- [2013h](tzdata2013h-tzvalidate.zip): fa8db4a6162d366d5ee87b9ab29e53f1ebac1ab832deb9133a2d6582443058ad
- [2013g](tzdata2013g-tzvalidate.zip): 09c6d77201c900ff288b6e53157189fd84ef6e48172557a335d8937287d2b2e5
- [2013f](tzdata2013f-tzvalidate.zip): 9cc74b7e4b1849bd8e0116f5af5e1f924560a271d29d3e29c4611e5222e8289c
- [2013e](tzdata2013e-tzvalidate.zip): 43b99f671eb4aabd59a0fbe59fb80497698aa589c0ebc80587d7392a90d45abf
- [2013d](tzdata2013d-tzvalidate.zip): b8b4e894301f140313071fe999498ac8db7843a4aacd9d95854494e06ce1f15e
- [2013c](tzdata2013c-tzvalidate.zip): 0208928582d94719b06ba5cf3b5977251670919b77d3789a984fa9a858faaef6
- [2013b](tzdata2013b-tzvalidate.zip): 2c7400398f7996076a53f0512396ee342f30d835bb9058f9c65266d39f297175
- [2013a](tzdata2013a-tzvalidate.zip): ed8f8c313142e543c79ffbb3dd87e3a7705bf6da50511f85459619f28ec6852a
- [2012j](tzdata2012j-tzvalidate.zip): 5376ef98de965e00c3e39b68f318fb56e8ef78a0bfee216104456d852c801b0a
- [2012i](tzdata2012i-tzvalidate.zip): c7afaa63409d7718813c5ae2d8038630cbdd0e327e89f608906d2cb8954a3cee
- [2012h](tzdata2012h-tzvalidate.zip): 6fb2c7c59c94032f9f7f17da26b7aea39810af8188d990c48676f8df676e3c91
- [2012g](tzdata2012g-tzvalidate.zip): 7950dcd917d2778ef78674f3e7bdc75480fcf9f6ecba8dc6661bbfcf748c5699
- [2012f](tzdata2012f-tzvalidate.zip): 72a4ce6f236de5be6c074f20c60541829de8bce29fea0ce2033495c624d97671
- [2012e](tzdata2012e-tzvalidate.zip): 2e83f88fee03d1899cdff4ebfb8aea4f5ef88bda7a2c5bba2f7cbbe467cefc64
- [2012d](tzdata2012d-tzvalidate.zip): fee5f481a378307e09f1862b98755dfd7554c917f38b15b59ff40e13215a22c9
- [2012c](tzdata2012c-tzvalidate.zip): 48bd240ebe3b7ebe74d7e5e6fc1206002b3cdd3505d747589946c01a65bd00c3
- [2012b](tzdata2012b-tzvalidate.zip): 78b1e9c34250c214fa18d08ae245377e693e90d44ddfc3ade523a2a15ba97312
- [2012a](tzdata2012a-tzvalidate.zip): 4d29f66849606e91fb480d7875c56e9db66d2b81dd1622783d1e6f064410be9c
- [2011n](tzdata2011n-tzvalidate.zip): 05d05167ea0339b43366e7c397f4421eb1149156573ac1347e35243a372171d2
- [2011m](tzdata2011m-tzvalidate.zip): a935eec442d17ef180a4b8c7292e18a634b7496c9072aa381d5126d3a1d6b17d
- [2011l](tzdata2011l-tzvalidate.zip): 0462390602fd5c7ce429a6825d04fe460a0a4c8593270fca082ee881d8b4b030
- [2011k](tzdata2011k-tzvalidate.zip): 11e227afb98aab18639d0feb84c22cb7f86ce4828bdc1028b68172d1c384a231
- [2011j](tzdata2011j-tzvalidate.zip): 45a907bd4455aec340c143dd9bdcb024cb09f118ecab47e2c48fb9e55caec713
- [2011i](tzdata2011i-tzvalidate.zip): c4ac40112b3d7182d176ba55a85364a833ffd8bebd2ee92f8b41bf5901aaf3ff
- [2011h](tzdata2011h-tzvalidate.zip): e9b38af60dce13a774eac45721d225966c14bda9446a629b9a8dc332e82fa130
- [2011g](tzdata2011g-tzvalidate.zip): 020cd72167462d0c38a98f5ae6228152c85c12513a01397518ac06dbc23a9baa
- [2011f](tzdata2011f-tzvalidate.zip): 579f876eb2483cdccc5f8f5973e428849c3adf9b91b51839692eee14968970e4
- [2011e](tzdata2011e-tzvalidate.zip): 829e4449704687b1cdb85e740f66b3a2dcceb592680c1feea3ffab3c218a7fca
- [2011d](tzdata2011d-tzvalidate.zip): e01f012d944ac1c3d5bf8f043868f75f220db9e94a69f2fd90e1e84faf579a53
- [2011c](tzdata2011c-tzvalidate.zip): b8fd6b29f40f48c8205da0928d083c850f072833828a57a537c11573363bc5ac
- [2011b](tzdata2011b-tzvalidate.zip): e64c32574c8875b0e9a9eb3fd1c9471ca77bd2f2872998032e26aeeaddc670e6
- [2011a](tzdata2011a-tzvalidate.zip): a1fe52623cb0040f5905c29cc97c499b2881675799c75173e9ce5f25ee1bdb60
- [2010o](tzdata2010o-tzvalidate.zip): 2e8dd0c4b8f865eef1d1027682492e83fb07dfac674a3c5b17fc3f6597aa4ae6
- [2010n](tzdata2010n-tzvalidate.zip): 1f227080b5313b6fb86cb54656d1e92f395590d768b052c99d791000714b16e9
- [2010m](tzdata2010m-tzvalidate.zip): 057a1c3144d6c81d41f90dd4b731dcf6ed6be54d85e2ceb54057bc28d14e2262
- [2010l](tzdata2010l-tzvalidate.zip): fc271a55fe7bcf6d14efa77981da661f5de00cba47df513df5501e8502b11361
- [2010k](tzdata2010k-tzvalidate.zip): 8f7739ec4a0264c970de5c8e6ee0e3e351d93971d7f079f4a4bb7805f8fc8a53
- [2010j](tzdata2010j-tzvalidate.zip): 76f416a670ab5478252613746a06c60d988e07a7204416b3eecccdd0596af65e
- [2010i](tzdata2010i-tzvalidate.zip): f0a51314344f610e874ae190db05d56a6207b644986cb1e168b3be9059b00620
- [2010h](tzdata2010h-tzvalidate.zip): 1a43e31c7fb0501cd25da9df4ecc84d3f0f577ef5661ec4e105357bdeda5839b
- [2010g](tzdata2010g-tzvalidate.zip): 87d5b9f40091eaf7d943085e7082440a9fb62263d414d79e8943c4b57933e948
- [2010f](tzdata2010f-tzvalidate.zip): a512769600ebbd0eccf0dc6ce9af416b2152ffee72c39df4ac5e2d1d72057ce6
- [2010e](tzdata2010e-tzvalidate.zip): 8d24c48e849ebaca350d8dea556696ebb3cb544574c8629ec0efceee346f0f02
- [2010d](tzdata2010d-tzvalidate.zip): 6d49c23b9d0618687fd461ad08e647b2179ca9ecd888ce442fba3c32f7171b57
- [2010c](tzdata2010c-tzvalidate.zip): 998f0414a36c9438dbd4bd06447a19d234493b446de5d15f8d36b4c962f3d6c2
- [2010b](tzdata2010b-tzvalidate.zip): a5b0f1235f91ce891a32e374d09e4c17ea73590935380fe49fb0dd2b3d9deeb1
- [2010a](tzdata2010a-tzvalidate.zip): c0f874a31a12c5bb8c47c244b73feb9643d06882aec195e508b8f5a9106b57e8
- [2009u](tzdata2009u-tzvalidate.zip): 9908a27a1ccdea0c23aa8b5419fafd5f6f14b8ef30097b4d69385b1c2f736ac9
- [2009t](tzdata2009t-tzvalidate.zip): 9ab6b3d239321583c5c78efcc7623dbe2bc7064f4debb8dd9334b301c010eba1
- [2009s](tzdata2009s-tzvalidate.zip): 9ab6b3d239321583c5c78efcc7623dbe2bc7064f4debb8dd9334b301c010eba1
- [2009r](tzdata2009r-tzvalidate.zip): e3e2b188547b59b5680f45561c77ce1c61606844ebc57fcbb0cc17217a0e9b04
- [2009q](tzdata2009q-tzvalidate.zip): 45b8dbad4a8c19daa7848d1d9ef72f3b55b56d1508847145460308a5905e5ce6
- [2009p](tzdata2009p-tzvalidate.zip): 093800357a6184dabfc8444178c3fdfa24c8de4936d54cef101160263b5c8b87
- [2009o](tzdata2009o-tzvalidate.zip): 31384df38e44055c016632504e90c49ee63edd3c7b66cffdc35b6450beec9213
- [2009n](tzdata2009n-tzvalidate.zip): 11bc29cdcb2d5f2cc6c552c05b7d6ae2561510493e40a8751d1dd7b2d89d4d0e
- [2009m](tzdata2009m-tzvalidate.zip): 573e07b9c556b7a296e9fcea4f3acb6757f580bf6a4f554e4e16c1af3a5a54cc
- [2009l](tzdata2009l-tzvalidate.zip): 44f03ec29bb6787b1eaa26dce5b91ff761b504e1d2bf8f61d287dbe1e34cdae4
- [2009k](tzdata2009k-tzvalidate.zip): 316b3e1f4272bacf25616708969bf57aecc3c349c169375750a488ae259881e6
- [2009j](tzdata2009j-tzvalidate.zip): 2caedcaa60b941890d91cd4f6a7ef8ef5d4d7101de6536cfaf002ea0c6d550f0
- [2009i](tzdata2009i-tzvalidate.zip): e3b8824634a59b92ef92d5c155ec73bfa77af0ff218c8e15aa12b66f30dfe9ae
- [2009h](tzdata2009h-tzvalidate.zip): bffc9b9e1bd4a6623a3286720108e21a259ea8f60395b2fd42b9cce53092d886
- [2009g](tzdata2009g-tzvalidate.zip): c58db467cbe2064107615b3d0ebe903a1cec0509e853f93c2f4aaf7c48dc4a8a
- [2009f](tzdata2009f-tzvalidate.zip): f57e2ca1e4e1e39fa7899f7668e5a81f3c04fbd2ba495b893b4de6db2ae33a60
- [2009e](tzdata2009e-tzvalidate.zip): 9a83a6c560a8468c7e7ae5a9ccdc41f074cf968d6e1b360d594185f38a238896
- [2009d](tzdata2009d-tzvalidate.zip): 024ecd0f289eb73fa2564af781a22cc6a0100364099710196835e0524ac02e36
- [2009c](tzdata2009c-tzvalidate.zip): aeae638431fdbb8ea7ee0383b822b74ecd076bc3bd0a6a1ab33bb9d742aee1d0
- [2009b](tzdata2009b-tzvalidate.zip): 65fbbc4ccebe866e641e971842b9ab415a07b1fb4da94f1290bc10f56f0b5b61
- [2009a](tzdata2009a-tzvalidate.zip): 65fbbc4ccebe866e641e971842b9ab415a07b1fb4da94f1290bc10f56f0b5b61
- [2008i](tzdata2008i-tzvalidate.zip): 6b216ac8a202661a4900ebf491d0d9a4f4b0de73fa0401b6d9f179637d3c5f6c
- [2008h](tzdata2008h-tzvalidate.zip): bd88609d3fd5cc058385c83650004e1e5a123613b5e3c4bacc731a19f322ac1e
- [2008g](tzdata2008g-tzvalidate.zip): 08a0b9458a2176880ef2173a46320a52751e737c112d4cfe59e1008af8f138eb
- [2008f](tzdata2008f-tzvalidate.zip): d60c600537b8a5d1909bea983777e56f4e2b80952b389dee8437139560445e90
- [2008e](tzdata2008e-tzvalidate.zip): f0b58d2caf9af776c60d53bef1f098abc484b7225dcda7d3d10ded65b777596a
- [2008d](tzdata2008d-tzvalidate.zip): dde4af93a44e88705a0aada9905c972c9c014075ec46ab111672022d6aeaefa4
- [2008c](tzdata2008c-tzvalidate.zip): 94e9ada6ffb17a03d3231708a9e64787c7bcec59ddc0cf8bc310c92a605c101d
- [2008b](tzdata2008b-tzvalidate.zip): 248a48b3d1f07abc2904a2f0e9ea9c8c180ccac15a2484921fc624b148a18afc
- [2008a](tzdata2008a-tzvalidate.zip): dcccd45072557a90cd8fe977e526b9144e46c81484724fc36848f3ff105dfa01
- [2007k](tzdata2007k-tzvalidate.zip): ed308d900c6c5db60315871d3121458f3f813ced7113234ece2829fa1367ebbb
- [2007j](tzdata2007j-tzvalidate.zip): ce53a23ab0e48e7e323c9482973d51138194aa10575b76d255adadcd687f86a1
- [2007i](tzdata2007i-tzvalidate.zip): e8e8a00f81a3548f3773190e709302f978edd20c91ca31a34fd63b702f0b0388
- [2007h](tzdata2007h-tzvalidate.zip): 1ea4657106269638763739047960710c94f55a9c9ca0307bc0bf531850336904
- [2007g](tzdata2007g-tzvalidate.zip): cfa638f4c01bf4941550db5f6e5bb8868b06ff1bc1545a3202d54b6609f94257
- [2007f](tzdata2007f-tzvalidate.zip): 6a93216498d3a14697aa3ddc02e4b84243f66ff21a3eee86df31a942eb2382c7
- [2007e](tzdata2007e-tzvalidate.zip): 348223997bb48d0cb24e80908477f90bc6bd526a2024500628a90c7056e23601
- [2007d](tzdata2007d-tzvalidate.zip): 429e9a087db3f96f2a74be8f0d5564bc2ea049a2aa4e4d4b55c55710ff4844a8
- [2007c](tzdata2007c-tzvalidate.zip): 08d835cd0857a32dc72292938d871620c5330922871841b0f5a2c03c4ac8d462
- [2007b](tzdata2007b-tzvalidate.zip): 7e71df7c9dab798c522025473f68c2035a3063abb68d976ca5e68d1e794e4b9d
- [2007a](tzdata2007a-tzvalidate.zip): 7e71df7c9dab798c522025473f68c2035a3063abb68d976ca5e68d1e794e4b9d
- [2006p](tzdata2006p-tzvalidate.zip): eb2b34f686b46d77935d2dfb4b5090fd6b25c86f72c86211493dd4517f4bd03a
- [2006o](tzdata2006o-tzvalidate.zip): 346f71e9cb5e8e8b193cfcb70a2a7a4a101354f0275e12a9adaf718a2fb64d54
- [2006n](tzdata2006n-tzvalidate.zip): ab42eea273698f7839b054ed75f574da24d82bdec29a40988c9c620c047c1f81
- [2006m](tzdata2006m-tzvalidate.zip): 772645fd1bd8f741e75b3e5b2326468c6340a458a92f1416d9a45c345f4a6044
- [2006l](tzdata2006l-tzvalidate.zip): 5aa8cd7443b0a90d92ff3f066a9b19dc4a519c7d3071c4749a2c39c69f63a4c5
- [2006k](tzdata2006k-tzvalidate.zip): 5ccab2b24f356f7dfaa9477598893fab4b57954836e5918c2ea82952bd7c1faf
- [2006j](tzdata2006j-tzvalidate.zip): 0266616becb7cf43464af9867d7b6814add3d64683a73bdd8baf3ba5b8986dad
- [2006g](tzdata2006g-tzvalidate.zip): bed7a40abadbc72a6923cd16da476885f538febc93131d4c604c4308e6462963
- [2006f](tzdata2006f-tzvalidate.zip): d844153ef8adfef10fe393858c7ea72eef142f34ee1f48e5e2d7d3036cb6dcb0
- [2006d](tzdata2006d-tzvalidate.zip): 4c24a28ce2493a1ab196cf485e19d6a7520d535c3613f64f075378b53be6c7c8
- [2006c](tzdata2006c-tzvalidate.zip): 525b6c653ca2fbabf2f4b6d3bd9e86c0a07ce9f5273ea2f5ef7d6d22a884e1d1
- [2006b](tzdata2006b-tzvalidate.zip): 356ba12e3b4d826e254d93076861b0953fd62c65884b636b8254cd3e112db59f
- [2006a](tzdata2006a-tzvalidate.zip): 356ba12e3b4d826e254d93076861b0953fd62c65884b636b8254cd3e112db59f
- [2005r](tzdata2005r-tzvalidate.zip): f6efaf4a67ce9427dc5a6ce7a232b66049db0291b8c9313b9101c6ab94a81930
- [2005q](tzdata2005q-tzvalidate.zip): 9ac0fe6bae3f97b3863d988777d15a3b92ef463951c14069f3ef0d8583932e47
- [2005p](tzdata2005p-tzvalidate.zip): 9ac0fe6bae3f97b3863d988777d15a3b92ef463951c14069f3ef0d8583932e47
- [2005o](tzdata2005o-tzvalidate.zip): 12291bba661e4655ea8d42dea5bd31a9f82d69e5edce0e9c28cce30c42cdee44
- [2005n](tzdata2005n-tzvalidate.zip): f81f4f5605dd6c8a55941e4fe5f2631ccb3686121f82e240a8a567778a926901
- [2005m](tzdata2005m-tzvalidate.zip): 7032eb7b5d4cadc3ca383f9da808f9e76e55a269444f13d1a85d1ed77767e8a9
- [2005k](tzdata2005k-tzvalidate.zip): 983fe01d8755303cf3e3ead8df5ab74b6066ee273471c81e666571624b2bfc00
- [2005j](tzdata2005j-tzvalidate.zip): 983fe01d8755303cf3e3ead8df5ab74b6066ee273471c81e666571624b2bfc00
- [2005i](tzdata2005i-tzvalidate.zip): 983fe01d8755303cf3e3ead8df5ab74b6066ee273471c81e666571624b2bfc00
- [2005h](tzdata2005h-tzvalidate.zip): c4cf675f65dcab102292666714f6b70002f9e92317cba7948247524a2e074d3f
- [2005g](tzdata2005g-tzvalidate.zip): 53c2e8dc3cc9ab3f23ac3a584a8b869c255d2c45b93c0b927736e1a664bdd076
- [2005f](tzdata2005f-tzvalidate.zip): 9626fff02ef0dcf9ec2d614260acab3477b2115221743946cea64379489b2fb6
- [2005e](tzdata2005e-tzvalidate.zip): 1ec9727182787a33c812b60905e8b1d00602e6ff45cf56da34da3104d147b077
- [2005c](tzdata2005c-tzvalidate.zip): 1ec9727182787a33c812b60905e8b1d00602e6ff45cf56da34da3104d147b077
- [2005b](tzdata2005b-tzvalidate.zip): f0e74082fcae359b444b6572a8daced23db2a406264fcd661881c5025a567641
- [2005a](tzdata2005a-tzvalidate.zip): f0e74082fcae359b444b6572a8daced23db2a406264fcd661881c5025a567641
- [2004g](tzdata2004g-tzvalidate.zip): f0e74082fcae359b444b6572a8daced23db2a406264fcd661881c5025a567641
- [2004e](tzdata2004e-tzvalidate.zip): 2d4e5e088af26853d344b46e18b22b4b5d544b9987d7c0e2d4a654011f050c8b
- [2004d](tzdata2004d-tzvalidate.zip): 3261e32014164fe09336b5026d95baafd5361e16ac0e636eb31307e70d9e0a94
- [2004b](tzdata2004b-tzvalidate.zip): 3e07041c06eda2668a8513e3baf72a2d16894a8289c4624fe61b38ba3004adaf
- [2004a](tzdata2004a-tzvalidate.zip): caf2217749b9adf4635efe9cae4d68430245e3028d4293e270f299e86ae2de15
- [2003e](tzdata2003e-tzvalidate.zip): d17750636f74c84567dd48741d60c00ccf64ac0661648467fe2cab4c8e325c79
- [2003d](tzdata2003d-tzvalidate.zip): 8da7a0882099756fdf223a5eb8d86c03b4e69e75e56bd6f3d670ca8c7bffb675
- [2003c](tzdata2003c-tzvalidate.zip): 90e21595fbd52a068dd9c4c7f6d8d3edafe378eee93bc8957f6b69885866acfe
- [2003b](tzdata2003b-tzvalidate.zip): 90e21595fbd52a068dd9c4c7f6d8d3edafe378eee93bc8957f6b69885866acfe
- [2003a](tzdata2003a-tzvalidate.zip): 90e21595fbd52a068dd9c4c7f6d8d3edafe378eee93bc8957f6b69885866acfe
- [2002d](tzdata2002d-tzvalidate.zip): d6953a7aabf2cd2fd8267d9231d444c346921de515e48acfc3c3d82cc8f995a3
- [2002c](tzdata2002c-tzvalidate.zip): af0f6e533f3b961ad2df4fe11906ed4f637d8859cebe2a23b9067e7576714c28
- [2002b](tzdata2002b-tzvalidate.zip): a6239a8219cfdf8d6fdc72ba3ee7f4aa94a5c2a669837c1adb4bf450eb037f63
- [2001d](tzdata2001d-tzvalidate.zip): acdef4b648273da85794df2781a74b137cab161629d381f1ce3c49197f9b000e
- [2001c](tzdata2001c-tzvalidate.zip): 03a8081d086b733bd7c6ccf1765a872bd14d1e29e4bd4fa106b230cd1a23343d
- [2001b](tzdata2001b-tzvalidate.zip): 6df9f8d3cf673009217bcb026adc3550e7396f90a3007d8e97543ed6b6b09580
- [2001a](tzdata2001a-tzvalidate.zip): 149539d208edfe1ead0bf130bc16cbc2301c7bcd40c68d494232b6a2b1096fec
- [2000h](tzdata2000h-tzvalidate.zip): 66ce0c00c63bae3f657677c0e13f9d1ea335ff73a68d976aa7316baf88ecb672
- [2000g](tzdata2000g-tzvalidate.zip): ca111b09df77ea72eb1bc98e263ae6f404e347a98e5038c928f5f76115f6fa44
- [2000f](tzdata2000f-tzvalidate.zip): 551f7435064f222090c863cfbfb9f9568795f4606d0ab33f8724913726b4df71
- [2000e](tzdata2000e-tzvalidate.zip): 88e91780c65faa68d1ecf7e5b43886cdd01560577cb426d1c1d4452bdc7bd26f
- [2000d](tzdata2000d-tzvalidate.zip): 3fec55684f069f81c52db5c95749ce8dd5daccf79add53e0f1f94f4189bbf4d0
- [2000c](tzdata2000c-tzvalidate.zip): 13eaa68fd5916b2addad6cd9cb7aee6f215d3f6a7e79821b5df6b00187988731
- [2000b](tzdata2000b-tzvalidate.zip): 6aef4077278de4d0892c778d32f310c2f97b128dd13767571a705af466e733ad
- [2000a](tzdata2000a-tzvalidate.zip): 3673e19ea83c62886ef349a8cfd930dc08e6dce9fd2add9216edbdc8dc24ea9a
- [1999j](tzdata1999j-tzvalidate.zip): 7dfaafa1fe41484ae032791e46bb99fac97548f218bdfcaf5bd44dc490927b63
- [1999i](tzdata1999i-tzvalidate.zip): 4db430065162225c5319723360597475ebbcb43fce828b1f962b212cad272b46
- [1999h](tzdata1999h-tzvalidate.zip): f304617e6cb00b3d5356bec07cb3d66d8ee191035dbe3718e1a14a21467283a8
- [1999g](tzdata1999g-tzvalidate.zip): 9be9b53a5cf5de2ab7f204af42cd7d4adcd8023ff5638e1af4080542e7373741
- [1999f](tzdata1999f-tzvalidate.zip): f31b1f2705e49970e73225d9ee6c6ae5d2ab22c1eb6a0b41e301100eac0b1ce2
- [1999e](tzdata1999e-tzvalidate.zip): c610a7ea1f2cda62acba77eed6a21638da2750e4f34d78b8c8973414c91b1f0b
- [1999d](tzdata1999d-tzvalidate.zip): 0b4d3b42b30641b83cb913681cfe1b975c75de9d3b72c6fd6c8188b2908ceca1
- [1999c](tzdata1999c-tzvalidate.zip): dfcc0463d417b5db025e7d52aec91b115334ba9e6371d4536592f55ae53b45c9
- [1999b](tzdata1999b-tzvalidate.zip): 0fe076b26cc089c2d3fd3f07ba0bd245ae1f6cd72229c921d90ea570a39bcbfd
- [1999a](tzdata1999a-tzvalidate.zip): 3772e1290c2215fbcd09cd27b577c5ef447984620aa059ec6d4d8b46e76c7f38
- [1998i](tzdata1998i-tzvalidate.zip): 3772e1290c2215fbcd09cd27b577c5ef447984620aa059ec6d4d8b46e76c7f38
- [1998h](tzdata1998h-tzvalidate.zip): 60ff21ea369a7b0c7955fc785538be49bb3e33afc63f06d640bf079317b62daa
- [1998e](tzdata1998e-tzvalidate.zip): 75c860323365cc57556e64809584493953398bbcf92db23bce7b33a6a8d98f96
- [1998d](tzdata1998d-tzvalidate.zip): fa302bf1e92dbf8dd8def4414c23216aa1271792901bc5dd90de689a197b3b50
- [1998c](tzdata1998c-tzvalidate.zip): fa302bf1e92dbf8dd8def4414c23216aa1271792901bc5dd90de689a197b3b50
- [1998b](tzdata1998b-tzvalidate.zip): fca095828e48ddb5c304de28cfd4a101369a07d01acfe180884c2623e7d35b45
- [1998a](tzdata1998a-tzvalidate.zip): fca095828e48ddb5c304de28cfd4a101369a07d01acfe180884c2623e7d35b45
- [1997k](tzdata1997k-tzvalidate.zip): bd044fb0d817a97ed1c814833ae081f5a5666f703cdb58beff497bd2817fedce
- [1997j](tzdata1997j-tzvalidate.zip): cb2a3d1089886635f1d9d6fce5f17ad0a42015f2e548f4685d721a48a847ecf9
- [1997i](tzdata1997i-tzvalidate.zip): cb2a3d1089886635f1d9d6fce5f17ad0a42015f2e548f4685d721a48a847ecf9
- [1997h](tzdata1997h-tzvalidate.zip): ed8e51187bef7dc1ff58dd9e7a025145655a936d39aa1fe42b34ae777f2d1da9
- [1997g](tzdata1997g-tzvalidate.zip): 5f7d09750069515129492d77bf8e8f849cde5566a195468589d8448d0a6f7a44
- [1997f](tzdata1997f-tzvalidate.zip): 696b173e0cf0a42dd2aa7e3a5f79480d5117a4b1656d8d4b05fd3da7838fbff9
- [1997e](tzdata1997e-tzvalidate.zip): 696b173e0cf0a42dd2aa7e3a5f79480d5117a4b1656d8d4b05fd3da7838fbff9
- [1997d](tzdata1997d-tzvalidate.zip): 8ac096cd9637313c4694eb58d37d8a102f3f4bed9ed6f1823824a98a885fff05
- [1997c](tzdata1997c-tzvalidate.zip): 8ac096cd9637313c4694eb58d37d8a102f3f4bed9ed6f1823824a98a885fff05
- [1997b](tzdata1997b-tzvalidate.zip): 46dfe0158608632f30e8b45b47d30714dc8d75b8e5f63b8841d12c28a8355076
- [1997a](tzdata1997a-tzvalidate.zip): 46dfe0158608632f30e8b45b47d30714dc8d75b8e5f63b8841d12c28a8355076
- [1996n](tzdata1996n-tzvalidate.zip): ccd001641f3fac20aa58d10f316478b5cc85431b973504c5973eafcb85750cec
- [1996l](tzdata1996l-tzvalidate.zip): d85e67b512a3e51481a8f23cc2ec8cebc78c6955c38076b135668f6b2f92c38d
- [96k](tzdata96k-tzvalidate.zip): d85e67b512a3e51481a8f23cc2ec8cebc78c6955c38076b135668f6b2f92c38d
- [96i](tzdata96i-tzvalidate.zip): 81c258d323b00e24cfb8c4d3a7f623ed611101b29853bacc6590ac191d95adab
- [96h](tzdata96h-tzvalidate.zip): f07813e5c83df515698a18e1218c2ca71e8e23659ee3b3f26979f6624b71fc48
- [96e](tzdata96e-tzvalidate.zip): 548edbb0b67846d099f74b1e14c731cd604f3f3ba8f4cfc2eec9b2bce36b8536
- [96d](tzdata96d-tzvalidate.zip): 142c73101f8930d88e9bce44d5d70fd9dbcfe8daaae079bd7a52f257f6c77c0f
- [96c](tzdata96c-tzvalidate.zip): 9c9fbbbf07558798eed039c73ebf164a35b91524f3dccdce57acd9ca5a390bb2
- [96b](tzdata96b-tzvalidate.zip): bc5bc6fc6659f72b811bb9cd0a821071ba0ef07a9234811c0a2ed3d7194002d1
- [96a](tzdata96a-tzvalidate.zip): 602959e56b669f7766f8691e758b88aabfb97b9a04db185a6dc30aa13ab856a1
- [95m](tzdata95m-tzvalidate.zip): 8c3e0a0449be0009d55ddba0cbac024967dfd5caa6a04d8c3ec54762b79f0bee
- [95l](tzdata95l-tzvalidate.zip): f56b2fdfe3f1a73203582e1b3d3f736daff03daa276d08af9ba230a93ba558a3
- [95k](tzdata95k-tzvalidate.zip): 49ae046334ebcfeac33aaa55aba0f17466fd5a86e925937dd001347b6a7f7a35
- [95i](tzdata95i-tzvalidate.zip): bc89e975c3ef824496554d56957415d92023f9a05fb2f5ebdbf303bf04f265e5
- [95h](tzdata95h-tzvalidate.zip): cd24079204851c26c3d8c93cefdcf6cf7df6a187b8ee15a58c09376fd9f20494
- [95g](tzdata95g-tzvalidate.zip): c101ea62ccab7dbfdd38035187822fe18690bc8a6c72c21d3242f562e9b211bc
- [95f](tzdata95f-tzvalidate.zip): c101ea62ccab7dbfdd38035187822fe18690bc8a6c72c21d3242f562e9b211bc
- [95e](tzdata95e-tzvalidate.zip): 64f05513e41f1d7bbc5e183446619f0066f610b25d4a3e921706c67a494225e5
- [95d](tzdata95d-tzvalidate.zip): 64f05513e41f1d7bbc5e183446619f0066f610b25d4a3e921706c67a494225e5
- [95c](tzdata95c-tzvalidate.zip): 7e8ec8309b306c7dfe4e018f296186a94a68dbd1a312c8dc70af3f5f677e8a6c
- [95b](tzdata95b-tzvalidate.zip): c74da6d6f23e697fb109bcd77ffc5160edc7fc5ac5144e1427107c6dbac1df62
- [94h](tzdata94h-tzvalidate.zip): eb6a94122e95e2e6f4a765fa8c1ca42b63cc60dc03a5a82051d50f5861c5a4e2
- [94f](tzdata94f-tzvalidate.zip): 731b61fb522f55bc348e109f3831c4b44cbee870f150dac8e3af494a0b9bc290
- [94e](tzdata94e-tzvalidate.zip): 3e743f5a52f41034ef50b1ff30575f984011f9f979f14a5c37a01d9ed98ac3fb
- [94d](tzdata94d-tzvalidate.zip): 9344ce3a1d16ef89aafffea2abd81e615534b5371cbb91c025a195bba90a4dd6
- [94b](tzdata94b-tzvalidate.zip): 6f19210706aa021e8183e15594e8f432be848eef75e48487d672e814cfeeef62
- [94a](tzdata94a-tzvalidate.zip): 6ab688304de2874ae0a9b6387b374f12a990ff983d8e1ef301f52f750c6b1b84
- [93g](tzdata93g-tzvalidate.zip): 6ab688304de2874ae0a9b6387b374f12a990ff983d8e1ef301f52f750c6b1b84
- [93f](tzdata93f-tzvalidate.zip): f5098176324ad2d41a02bc943524a9642920c7a06119dc0768b1c1a6d87a43a1
- [93e](tzdata93e-tzvalidate.zip): cf88b4fa61e915beff3c949882300801e05435852d6ab0ed4621cb44aa3de49e
- [93d](tzdata93d-tzvalidate.zip): bbbb2143ac95d8f09e56833d262a983f25bd664241975d0ab67aba2bdabbdb87
- [93c](tzdata93c-tzvalidate.zip): 8b895cf900de3460780c9b280e8e74fde4125f04e8e841db2a028bc78bb43523
- [93b](tzdata93b-tzvalidate.zip): 9668a036cf2cb73e1f0c9159b1b733f4036583c3bc686a33cb6ba1e33547d78d
