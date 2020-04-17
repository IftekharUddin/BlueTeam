from l33t import generate_all_l33t

with open('profane.txt', 'rt') as fp:
    profane_words = fp.read().split('\n')

res = []
for word in profane_words:
    for l33t in generate_all_l33t(word, mode='all'):
        res.append(l33t)

with open('l33t_profane.txt', 'wt') as fp:
    fp.write('\n'.join(res))