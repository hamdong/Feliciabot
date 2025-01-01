namespace Feliciabot.net._6._0.models
{
    public static class Responses
    {
        public static readonly string[][] RollResponses =
        [
            [
                "As I see it, yes",
                "It is certain!",
                "Most likely!",
                "Outlook good!",
                "Signs point to yes",
                "Without a doubt",
                "Yes",
                "Yes - definitely",
            ],
            [
                "Don't count on it",
                "My reply is no",
                "My sources say no",
                "Outlook not so good",
                "Very doubtful",
            ],
            [
                "Reply hazy, try again",
                "Ask again later",
                "Better not tell you now",
                "Cannot predict now",
                "Concentrate and ask again",
            ],
        ];

        public static readonly string[] TeaResponses =
        [
            "Honey",
            "Green",
            "Oolong",
            "White",
            "Yellow",
            "Mulberry",
            "Herbal",
            "Chai",
            "Ceylon",
            "Mint ginger lemon",
            "Chamomile with lemongrass",
            "Spearmint",
            "Peppermint",
            "Chamomile",
            "Mulberry",
            "Hibiscus",
            "Jasmine",
            "Rooibos",
            "Milk",
            "Frozen Milk",
        ];

        public static readonly string[] GreetingResponses =
        [
            "Nice to see you!",
            "Can I get you something?",
            "I'll do my best to be of service!",
            "Whoa! Wh-whoa! Whoa!",
            "We've got trouble!",
            "Didn't expect that, did ya?",
            "All right!",
            "Hiho!",
            "Greetings, comrade!",
            "Welcome to maid hell, enjoy your stay!",
            "OwO!",
            "Here goes nothing!",
            "Yay!",
        ];

        private static readonly string DISCORD_CDN = "https://cdn.discordapp.com/emojis/";
        public static readonly string[] DanceResponses =
        [
            $"{DISCORD_CDN}899319530269061161.gif", // Marianne
            $"{DISCORD_CDN}1323820425439871047.gif", // Kirby
            $"{DISCORD_CDN}1322694789124325498.gif", // Nekoarc
            $"{DISCORD_CDN}1323819972253847582.gif", // Corrin
            $"{DISCORD_CDN}1323837299221463132.gif", // Hutao
            $"{DISCORD_CDN}1324007932035334225.gif", // Nezuko
        ];
    }
}
