# SpiroPlay
A suite-of-games breathing game app for children with asthma, including a first rule-based version for fault detection. For instructions of use see https://youtu.be/MSPVP-FCaYc.

The projects starts from the point we have gathered the realtime flow data from a handheld spirometer device (through our own middleware which we can't release). The expected stream contains a flow per 10-ish ms. Various handheld spirometer devices can deliver such a stream, although it is important to have both measures for inhalation and exhalation. 

11 metaphors are included, one (fishing) is currently not yet functioning and should be kept locked. 

We realease the software under CC BY 4.0 Adaptations and use of (parts of) the software is allowed with appropriate attribution, following https://creativecommons.org/licenses/by/4.0/. Attribution and changes should always be included in a paper and software making use of the whole or parts of this project. If you do integrate it or publish based on this work, we would appreciate a notification, this later is not part of the license. 

For research this includes a clear reference to our first paper: https://dl.acm.org/doi/10.1145/3410404.3414223 publicly available after embargo https://research.utwente.nl/en/publications/spiroplay-a-suite-of-breathing-games-for-spirometry-by-kids-amp-e. 

For commercial applications attribution can be included in a splash screen, home screen, or about page; we urge the adaptor to include it within the executable accesible to the end-user. 

The project files are to be openen in Unity version 2019.3.2f1. Some parts of the project have dependencies on 3rd party Unity Assets, we can't share these parts of the project. This also means that the project is likely to result in build errors when just using this version.
We might find a way to help you to re-include these parts, in order to do this contact us personally. 
We will ask verification of having the following third party assets linked to the involved Unity accounts: 
- Soft Alpha UI mask (https://assetstore.unity.com/packages/tools/particles-effects/soft-alpha-ui-mask-50339)
- Confetti GFX (https://assetstore.unity.com/packages/vfx/particles/confetti-fx-82497)
- Awesome Charts and Graphs (https://assetstore.unity.com/packages/tools/gui/awesome-charts-and-graphs-138153)

The visuals of the responsive metaphors can be found here https://www.youtube.com/watch?v=v008TQ3_FsA.

Regards,
van Delden, Robby

Created by:
Vogel, Koen

Building on development work of:
(BLE) networking: Smid, Kevin and Klaassen, Randy
fault detection and improvements: de With, Vivianne and Matienne van der Kamp
co-design basis: thanks to the children of BSO de Vlinder; van Delden, Robby; Zwart, Nynke; and Danny Plass - Oude Bos 
metaphor creation: Sil Tijssen, Rens van der Werff
graphics: Sterre van Arum (original), and Diana-Valentina Focsaneanu
