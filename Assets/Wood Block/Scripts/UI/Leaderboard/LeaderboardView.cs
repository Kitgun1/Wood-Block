using System.Collections.Generic;
using System.Linq;
using Agava.YandexGames;
using KimicuUtility;
using UnityEngine;

namespace WoodBlock
{
    public class LeaderboardView : MonoBehaviour
    {
        [SerializeField] private LeaderboardItem _leaderboardItem;
        [SerializeField] private string _leaderboardName = "leaderboard";
        [SerializeField] private GameObject _parent;

        private readonly List<LeaderboardItem> _items = new();

        private void OnGetEntriesError(string error)
        {
            Debug.LogWarning(error);
        }

        private void OnGetEntriesSuccess(LeaderboardGetEntriesResponse response)
        {
            var entries = response.entries.OrderBy(e => e.rank).ToList();
            foreach (LeaderboardEntryResponse entry in entries)
            {
                LeaderboardItem item = Instantiate(_leaderboardItem, _parent.transform);
                item.NameProfile.text = entry.player.publicName;
                item.Score.text = entry.score.ToString();
                StartCoroutine(entry.player.profilePicture
                    .GetPicture(texture2D => OnGetPictureSuccess(texture2D, item)));
                _items.Add(item);
            }
        }

        private void OnGetPictureSuccess(Texture2D texture2D, LeaderboardItem item)
        {
            Rect rectTexture = new(0, 0, texture2D.width, texture2D.height);
            item.Picture.color = Color.white;
            item.Picture.sprite = Sprite.Create(texture2D, rectTexture, Vector2.one / 2);
        }

        public void ClearLeaderboard()
        {
            foreach (LeaderboardItem item in _items)
            {
                Destroy(item.gameObject);
            }

            _items.Clear();
        }

        public void LeaderboardShow()
        {
            ClearLeaderboard();
#if !UNITY_EDITOR && UNITY_WEBGL
            Leaderboard.GetEntries(_leaderboardName, OnGetEntriesSuccess, OnGetEntriesError, 10,
                pictureSize: ProfilePictureSize.small);
#else
            List<LeaderboardEntryResponse> entries = new();
            for (int i = 1; i < 11; i++)
            {
                entries.Add(new LeaderboardEntryResponse
                {
                    player = new PlayerAccountProfileDataResponse
                    {
                        profilePicture =
                            "https://avatars.mds.yandex.net/get-yapic/58107/M5aCCLYNdG7knQtlnX9FgPcTs-1/islands-200",
                        publicName = "Kimicu",
                        lang = "ru"
                    },
                    extraData = "",
                    score = i,
                    rank = 11 - i
                });
            }

            LeaderboardGetEntriesResponse response = new()
            {
                entries = entries.ToArray(),
                userRank = 1
            };
            OnGetEntriesSuccess(response);
#endif
        }
    }
}