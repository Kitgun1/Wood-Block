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
        [SerializeField] private LeaderboardItem _leaderboardItemMe;
        [SerializeField] private string _leaderboardName = "leaderboard";
        [SerializeField] private GameObject _parent;

        private readonly List<LeaderboardItem> _items = new();

        #region Errors

        private void OnGetEntriesError(string error)
        {
            Debug.LogWarning(error);
        }

        private void OnGetProfileEntryError(string error)
        {
            Debug.LogWarning(error);
        }

        #endregion

        private void OnGetEntriesSuccess(LeaderboardGetEntriesResponse response)
        {
            var entries = response.entries.OrderBy(e => e.rank).ToList();
            foreach (LeaderboardEntryResponse entry in entries)
            {
                LeaderboardItem item = Instantiate(entry.rank == response.userRank
                    ? _leaderboardItemMe
                    : _leaderboardItem, _parent.transform);

                item.NameProfile.text = entry.player.publicName;
                item.Score.text = entry.score.ToString();
                StartCoroutine(entry.player.profilePicture
                    .GetPicture(texture2D => OnGetPictureSuccess(texture2D, item)));
                _items.Add(item);
            }
        }

        private void OnGetProfileEntrySuccess(LeaderboardEntryResponse response)
        {
            _leaderboardItemMe.NameProfile.text = response.player.publicName;
            _leaderboardItemMe.Score.text = response.score.ToString();
            StartCoroutine(response.player.profilePicture
                .GetPicture(texture2D => OnGetPictureSuccess(texture2D, _leaderboardItemMe)));
        }

        private void OnGetPictureSuccess(Texture2D texture2D, LeaderboardItem item)
        {
            Rect rectTexture = new(0, 0, texture2D.width, texture2D.height);
            item.Picture.color = Color.white;
            item.Picture.sprite = Sprite.Create(texture2D, rectTexture, Vector2.one / 2);
        }

        private List<LeaderboardEntryResponse> GenerateRandomEntries()
        {
            List<LeaderboardEntryResponse> result = new();
            for (int i = 1; i < 11; i++)
            {
                result.Add(new LeaderboardEntryResponse
                {
                    player = new PlayerAccountProfileDataResponse
                    {
                        profilePicture =
                            "https://avatars.mds.yandex.net/get-yapic/58107/M5aCCLYNdG7knQtlnX9FgPcTs-1/islands-200",
                        publicName = $"Other player {Random.Range(0, 9999)}",
                        lang = "ru"
                    },
                    extraData = "",
                    score = i,
                    rank = 11 - i
                });
            }

            return result;
        }

        private LeaderboardEntryResponse GenerateRandomProfile(int rank)
        {
            return new()
            {
                player = new PlayerAccountProfileDataResponse
                {
                    profilePicture =
                        "https://avatars.mds.yandex.net/get-yapic/58107/M5aCCLYNdG7knQtlnX9FgPcTs-1/islands-200",
                    publicName = $"Kimicu {Random.Range(0, 9999)}",
                    lang = "ru"
                },
                rank = rank,
                extraData = "empty",
                score = Random.Range(0, 199),
            };
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
            Leaderboard.GetPlayerEntry(_leaderboardName, OnGetProfileEntrySuccess, OnGetProfileEntryError,
                pictureSize: ProfilePictureSize.small);
#else
            int userRank = Random.Range(1, 15);
            LeaderboardGetEntriesResponse response = new()
            {
                entries = GenerateRandomEntries().ToArray(),
                userRank = userRank
            };

            OnGetEntriesSuccess(response);
            OnGetProfileEntrySuccess(GenerateRandomProfile(userRank));
#endif
        }
    }
}